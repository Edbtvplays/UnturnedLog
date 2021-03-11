// ReSharper disable AnnotateNotNullParameter
// ReSharper disable AnnotateNotNullTypeMember

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using SDG.Unturned;
using OpenMod.Unturned.Players.Connections.Events;
using OpenMod.Unturned.Players.Life.Events;
using OpenMod.Unturned.Players.Stats.Events;
using OpenMod.Unturned.Players.Chat.Events;
using OpenMod.Unturned.Players.Bans.Events;
using HarmonyLib;
using Edbtvplays.UnturnedLog.Unturned.API.Classes;
using Edbtvplays.UnturnedLog.Unturned.API.Classes.SteamWebApiClasses;
using Edbtvplays.UnturnedLog.Unturned.API.Services;
using Steamworks;
using UnityEngine;
using OpenMod.Unturned.Players;

namespace Edbtvplays.UnturnedLog
{   

    public class UserEventsListener : IEventListener<UnturnedPlayerConnectedEvent>, IEventListener<UnturnedPlayerDisconnectedEvent>, IEventListener<UnturnedPlayerDeathEvent>, IEventListener<UnturnedPlayerChattingEvent>, IEventListener<UnturnedPlayerBannedEvent>, IEventListener<UnturnedPlayerStatIncrementedEvent>
    {
        private readonly IUnturnedLogRepository m_UnturnedLogRepository;
        private readonly IConfiguration m_Configuration;


        // Initializing the Class
        public UserEventsListener(IUnturnedLogRepository UnturnedLogRepository, IConfiguration configuration)
        {
            m_UnturnedLogRepository = UnturnedLogRepository;
            m_Configuration = configuration;

            Subscribe();
        }


        // On player connected 
        public async Task HandleEventAsync(object sender, UnturnedPlayerConnectedEvent @event)
        {
            var player = @event.Player;
            var playerId = player.SteamPlayer.playerID;
            var steamId = player.SteamId;

            // Getting Steam Group information
            var pfpHash = await GetProfilePictureHashAsync(steamId);
            var groupName = await GetSteamGroupNameAsync(playerId.group);
            var hwid = string.Join("", playerId.hwid);
            if (!player.SteamPlayer.transportConnection.TryGetIPv4Address(out var ip))
                ip = uint.MinValue;
            // Quest Status
            var questGroupId = player.Player.quests.groupID.m_SteamID;

            var pData = await m_UnturnedLogRepository.FindPlayerAsync(steamId.ToString(), UserSearchMode.FindById); // returns user in database
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                         await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            if (pData == null) // If user does not exsist in database. 
            {
                pData = EventDatabase.BuildPlayerData(steamId.m_SteamID, player.SteamPlayer.playerID.characterName,
                    playerId.playerName, hwid, ip,
                    pfpHash, questGroupId, playerId.group.m_SteamID, groupName, 0, 0,
                    DateTime.Now, server);

                await m_UnturnedLogRepository.AddPlayerDataAsync(pData);
            }
            else // If user does exsist in database. 
            {
                pData.ProfilePictureHash = pfpHash;
                pData.CharacterName = player.SteamPlayer.playerID.characterName;
                pData.Hwid = hwid;
                pData.Ip = ip;
                pData.LastLoginGlobal = DateTime.Now;

                if (questGroupId != 0)
                    pData.LastQuestGroupId = questGroupId;

                pData.SteamGroup = playerId.group.m_SteamID;
                pData.SteamGroupName = groupName;
                pData.SteamName = playerId.playerName;

                pData.Server = server;
                pData.ServerId = server.Id;

                await m_UnturnedLogRepository.SaveChangesAsync();
            }

            var Eventdisconnected = EventDatabase.BuildEventData(pData, "Connected", "", server);

            await m_UnturnedLogRepository.AddPlayerEventAsync(Eventdisconnected);
        }

        // On player disconnected
        public async Task HandleEventAsync(object sender, UnturnedPlayerDisconnectedEvent @event)
        {
            var player = @event.Player;
            var playerId = player.SteamPlayer.playerID;
            var steamId = player.SteamId;
            var pfpHash = await GetProfilePictureHashAsync(steamId);
            var groupName = await GetSteamGroupNameAsync(playerId.group);
            var hwid = string.Join("", playerId.hwid);
            if (!player.SteamPlayer.transportConnection.TryGetIPv4Address(out var ip))
                ip = uint.MinValue;

            var pData = await m_UnturnedLogRepository.FindPlayerAsync(player.SteamId.ToString(), UserSearchMode.FindById);
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                         await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();


            if (pData == null)
            {
                pData = EventDatabase.BuildPlayerData(steamId.m_SteamID, player.SteamPlayer.playerID.characterName,
                    playerId.playerName, hwid, ip,
                    pfpHash, player.Player.quests.groupID.m_SteamID, playerId.group.m_SteamID, groupName, 0, 0,
                    DateTime.Now, server);

                await m_UnturnedLogRepository.AddPlayerDataAsync(pData);

            }
            else
            {
                pData.ProfilePictureHash = pfpHash;
                pData.LastQuestGroupId = player.Player.quests.groupID.m_SteamID;
                pData.SteamGroup = playerId.group.m_SteamID;
                pData.SteamGroupName = groupName;
                pData.SteamName = playerId.playerName;
                pData.TotalPlaytime += DateTime.Now.Subtract(pData.LastLoginGlobal).TotalSeconds;

                await m_UnturnedLogRepository.SaveChangesAsync();
            }

            var Eventdisconnected = EventDatabase.BuildEventData(pData, "Disconnected", "", server);

            await m_UnturnedLogRepository.AddPlayerEventAsync(Eventdisconnected);
        }


        //  On Player Death also adds kills 
        public async Task HandleEventAsync(object sender, UnturnedPlayerDeathEvent @event)
        {

            var playerwhodied = @event.Player; // gets the player who died to update there information 

            // Gets current Player
            var player = await m_UnturnedLogRepository.FindPlayerAsync(playerwhodied.SteamId.ToString(), UserSearchMode.FindById);
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                         await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            var EventDataDeath = EventDatabase.BuildEventData(player, "Death", "Reason: Unknown", server);

            await m_UnturnedLogRepository.AddPlayerEventAsync(EventDataDeath);


            var instigator = @event.Instigator;

            var instigatorplayer = await m_UnturnedLogRepository.FindPlayerAsync(instigator.ToString(), UserSearchMode.FindById);

            // TODO: Fix Player Kill for Suicide and to Put the killer in the Player

            if (instigatorplayer != null) // If the event instigator for the Death event, is a player. 
            {
                var EventDataKill = EventDatabase.BuildEventData(instigatorplayer, "Player Kill", "Player: " + instigatorplayer, server);
                await m_UnturnedLogRepository.AddPlayerEventAsync(EventDataKill);
            }
        }

        // On player CHat message.
        public async Task HandleEventAsync(object sender, UnturnedPlayerChattingEvent @event)
        {
            var eventplayer = @event.Player;

            // Gets current Player

            var player = await m_UnturnedLogRepository.FindPlayerAsync(eventplayer.SteamId.ToString(), UserSearchMode.FindById);
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                         await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            var EventDataDeath = EventDatabase.BuildEventData(player, "Chat Message", "Content: " + @event.Message, server);

            await m_UnturnedLogRepository.AddPlayerEventAsync(EventDataDeath);

            // Set the stuff to be saved into the pdata object to be saved into the database Only stuff that is modified here needs to be set
        }



        public async Task HandleEventAsync(object sender, UnturnedPlayerStatIncrementedEvent @event)
        {
            var player = @event.Player;

            var Event = @event.Stat;

            var Eventplayer = await m_UnturnedLogRepository.FindPlayerAsync(player.SteamId.ToString(), UserSearchMode.FindById);
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                         await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            if (Event == EPlayerStat.KILLS_ANIMALS)
            {
                var PlayerEvent = EventDatabase.BuildEventData(Eventplayer, "Killed Animal", " ", server);
                await m_UnturnedLogRepository.AddPlayerEventAsync(PlayerEvent);

            } 
            else if (Event == EPlayerStat.KILLS_ZOMBIES_NORMAL)
            {

                var PlayerEvent = EventDatabase.BuildEventData(Eventplayer, "Killed Zombie", " ", server);
                await m_UnturnedLogRepository.AddPlayerEventAsync(PlayerEvent);
            }
            else if (Event == EPlayerStat.HEADSHOTS)
            {

                var PlayerEvent = EventDatabase.BuildEventData(Eventplayer, "Player Headshot", " ", server);
                await m_UnturnedLogRepository.AddPlayerEventAsync(PlayerEvent);
            }

        }

        public async Task HandleEventAsync(object sender, UnturnedPlayerBannedEvent @event)
        {
            var player = @event.BannedPlayer;

            // Gets current Player
            var pData = await m_UnturnedLogRepository.FindPlayerAsync(player.ToString(), UserSearchMode.FindById);
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            // TODO: Need to fix the Reason and Instigator if executed by console. Check if Instigator = 0 and Reason = 0.
            var Eventpunnished = EventDatabase.BuildEventData(pData, "Player Banned", "Reason: " + @event.Reason + " Banned By: " + @event.Instigator, server);

            await m_UnturnedLogRepository.AddPlayerEventAsync(Eventpunnished);

        }



        // Gets player profile picture 
        private async Task<string> GetProfilePictureHashAsync(CSteamID user)
        {
            var apiKey = m_Configuration["steamWebApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                return "";

            using var web = new WebClient();
            var result =
                await web.DownloadStringTaskAsync(
                    $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={apiKey}&steamids={user.m_SteamID}");

            var deserialized = JsonConvert.DeserializeObject<PlayerSummaries>(result);

            return deserialized.response.players
                       .FirstOrDefault(k => k.steamid.Equals(user.ToString(), StringComparison.Ordinal))?.avatarhash ??
                   "";
        }


        // Gets players steam group
        private static async Task<string> GetSteamGroupNameAsync(CSteamID groupId)
        {
            using var web = new WebClient();
            var result =
                await web.DownloadStringTaskAsync("http://steamcommunity.com/gid/" + groupId +
                                                  "/memberslistxml?xml=1");

            if (!result.Contains("<groupName>") || !result.Contains("</groupName>")) return "";

            var start = result.IndexOf("<groupName>", 0, StringComparison.Ordinal) + "<groupName>".Length;
            var end = result.IndexOf("</groupName>", start, StringComparison.Ordinal);

            var data = result.Substring(start, end - start);
            data = data.Trim();
            data = data.Replace("<![CDATA[", "").Replace("]]>", "");
            return data;
        }


        public void Subscribe()
        {
            UnturnedPatches.OnResourceKill += Events_OnResourceKill;
            UnturnedPatches.OnresourceDamagedEvent += Events_OnResourceDamaged;
        }

        // Unsubscribes to the Event
        public void UnSubscribe()
        {
            UnturnedPatches.OnResourceKill -= Events_OnResourceKill;
            UnturnedPatches.OnresourceDamagedEvent -= Events_OnResourceDamaged;
        }

        public async void Events_OnResourceKill(SDG.Unturned.Player player, ResourceAsset asset)
        {
            var instigatorplayer = await m_UnturnedLogRepository.FindPlayerAsync(player.ToString(), UserSearchMode.FindById);
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
             await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            var EventTreeCut = EventDatabase.BuildEventData(instigatorplayer, "Tree Cut", " ", server);

            await m_UnturnedLogRepository.AddPlayerEventAsync(EventTreeCut);
        }

        public async void Events_OnResourceDamaged(SDG.Unturned.Player player, SDG.Unturned.ResourceSpawnpoint asset)
        {
            var instigatorplayer = await m_UnturnedLogRepository.FindPlayerAsync(player.ToString(), UserSearchMode.FindById);
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
             await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            var EventTreeCut = EventDatabase.BuildEventData(instigatorplayer, "Harvested Node", " ", server);

            await m_UnturnedLogRepository.AddPlayerEventAsync(EventTreeCut);
        }

    }

    class EventDatabase
    {
        public static PlayerData BuildPlayerData(ulong steamId, string characterName, string steamName, string hwid, uint ip,
            string profileHash, ulong questGroup, ulong steamGroup, string steamGroupName, double totalPlaytime, int Deaths,
            DateTime lastLogin, Server server)
        {
            return new PlayerData
            {
                Id = steamId,
                CharacterName = characterName,
                SteamName = steamName,
                Hwid = hwid,
                Ip = ip,
                ProfilePictureHash = profileHash,
                LastQuestGroupId = questGroup,
                SteamGroup = steamGroup,
                SteamGroupName = steamGroupName,
                TotalPlaytime = totalPlaytime,
                LastLoginGlobal = lastLogin,
                ServerId = server.Id, // Sets server Id from the returned server object 
                Server = server
            };
        }

        public static PlayerEvents BuildEventData(PlayerData player, string etype, string edata, Server server)
        {
            return new PlayerEvents
            {
                PlayerId = player.Id,
                EventType = etype,
                EventData = edata,
                ServerId = server.Id,
                Player = player,
                Server = server,
                EventTime = DateTime.UtcNow
            };
        }
    }

    // Harmony patch class for some eventst hat are not avalible in the base api. Such as nodes cut down trees mind etc... 
    public class UnturnedPatches
    {

        public delegate void ResourceKill(SDG.Unturned.Player player, ResourceAsset asset);

        public static event ResourceKill? OnResourceKill;

        public delegate void ResourceDamaged(SDG.Unturned.Player player, ResourceSpawnpoint asset);

        public static event ResourceDamaged? OnresourceDamagedEvent;

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(PlayerQuests), "trackTreeKill")]
            [HarmonyPostfix]
            private static void TrackResourceKill(PlayerQuests __instance, Guid treeGuid)
            {
                // You can get the resource asset from the guid
                var asset = (ResourceAsset)Assets.find(treeGuid);

                // The native player
                SDG.Unturned.Player player = __instance.player;

                OnResourceKill?.Invoke(player, asset);
            }

            private static void OnResourceDamaged(CSteamID instigatorSteamID, Transform objectTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
            {
                // Get the node
                if (!ResourceManager.tryGetRegion(objectTransform, out byte x, out byte y, out ushort index)) return;
                ResourceSpawnpoint resourceDamaged = ResourceManager.getResourceSpawnpoint(x, y, index);

                // Check if node is going to die.
                if (pendingTotalDamage < resourceDamaged.health) return;


                SDG.Unturned.Player player = PlayerTool.getPlayer(instigatorSteamID);

                // Player mined a node
                OnresourceDamagedEvent?.Invoke(player, resourceDamaged);
            }
        }
    }
}