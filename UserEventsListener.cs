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
using OpenMod.Unturned.Players.Movement.Events;

namespace Edbtvplays.UnturnedLog
{   

    // Class user event listener inherits Openmods Eventlistener with the paparementers of the events we wish to use. 
    public class UserEventsListener : IEventListener<UnturnedPlayerConnectedEvent>, IEventListener<UnturnedPlayerDisconnectedEvent>, IEventListener<UnturnedPlayerDeathEvent>, IEventListener<UnturnedPlayerChattingEvent>, IEventListener<UnturnedPlayerBannedEvent>, IEventListener<UnturnedPlayerStatIncrementedEvent>, IEventListener<UnturnedPlayerTeleportingEvent>
    {
        private readonly IUnturnedLogRepository m_UnturnedLogRepository;
        private readonly IConfiguration m_Configuration;


        // Initializing the Class
        public UserEventsListener(IUnturnedLogRepository UnturnedLogRepository, IConfiguration configuration)
        {
            // Uses the Repository for the plugin implementation aswell as the configuration file. 
            m_UnturnedLogRepository = UnturnedLogRepository;
            m_Configuration = configuration;
        }


        // On player connected 
        public async Task HandleEventAsync(object sender, UnturnedPlayerConnectedEvent @event)
        {
            var player = @event.Player;
            var playerId = player.SteamPlayer.playerID;
            var steamId = player.SteamId;

            // Getting Steam Information
            var pfpHash = await GetProfilePictureHashAsync(steamId);
            var groupName = await GetSteamGroupNameAsync(playerId.group);

            // Getting the hardware ID
            var hwid = string.Join("", playerId.hwid);

            // Retrieving the playuers IP
            if (!player.SteamPlayer.transportConnection.TryGetIPv4Address(out var ip))
                ip = uint.MinValue; // Changes the IP into a storable value 
            
            // Quest Status
            var questGroupId = player.Player.quests.groupID.m_SteamID;

            // Find player using there steam ID
            var pData = await m_UnturnedLogRepository.FindPlayerAsync(steamId.ToString(), UserSearchMode.FindById); // returns user in database

            // Finds the current server and if it doesnt exsist register current server. 
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                         await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            if (pData == null) // If user does not exsist in database. 
            {

                // Costruct the new user object using build playerdata 
                pData = EventDatabase.BuildPlayerData(steamId.m_SteamID, player.SteamPlayer.playerID.characterName,
                    playerId.playerName, hwid, ip,
                    pfpHash, questGroupId, playerId.group.m_SteamID, groupName, 0, 0,
                    DateTime.Now, server);

                // which is then added to the database. 
                await m_UnturnedLogRepository.AddPlayerDataAsync(pData);
            }
            else // If user does exsist in database. 
            {   
                // Set the stuff needed in pdata manually overwriting what was in there previously using the datat gathered above. 
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

                // save changes in the datatabase 
                await m_UnturnedLogRepository.SaveChangesAsync();
            }
                
            // Constructs a connected event 
            var Eventdisconnected = EventDatabase.BuildEventData(pData, "Connected", "", server);

            // Adds event to the database. 
            await m_UnturnedLogRepository.AddPlayerEventAsync(Eventdisconnected);
        }

        // On player disconnected
        public async Task HandleEventAsync(object sender, UnturnedPlayerDisconnectedEvent @event)
        {
            var player = @event.Player;
            var playerId = player.SteamPlayer.playerID;
            var steamId = player.SteamId;

            // Getting Steam Information
            var pfpHash = await GetProfilePictureHashAsync(steamId);
            var groupName = await GetSteamGroupNameAsync(playerId.group);

            // Getting the Hardware ID
            var hwid = string.Join("", playerId.hwid);

            // Trying to get the IPV4 Address
            if (!player.SteamPlayer.transportConnection.TryGetIPv4Address(out var ip))
                ip = uint.MinValue; // turns the IP into a storeable value

            // Searches database using the steam ID 
            var pData = await m_UnturnedLogRepository.FindPlayerAsync(player.SteamId.ToString(), UserSearchMode.FindById);

            // Gets the current server
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                         await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            // If player isnt in the players table
            if (pData == null)
            {
                pData = EventDatabase.BuildPlayerData(steamId.m_SteamID, player.SteamPlayer.playerID.characterName,
                    playerId.playerName, hwid, ip,
                    pfpHash, player.Player.quests.groupID.m_SteamID, playerId.group.m_SteamID, groupName, 0, 0,
                    DateTime.Now, server);

                await m_UnturnedLogRepository.AddPlayerDataAsync(pData);

            }
            // If player is in the player table 
            else
            {
                // Overwrite values in the object
                pData.ProfilePictureHash = pfpHash;
                pData.LastQuestGroupId = player.Player.quests.groupID.m_SteamID;
                pData.SteamGroup = playerId.group.m_SteamID;
                pData.SteamGroupName = groupName;
                pData.SteamName = playerId.playerName;
                pData.TotalPlaytime += DateTime.Now.Subtract(pData.LastLoginGlobal).TotalSeconds;

                // Save changes to the database.
                await m_UnturnedLogRepository.SaveChangesAsync();
            }

            // constructs a disconnected event
            var Eventdisconnected = EventDatabase.BuildEventData(pData, "Disconnected", "", server);

            // saves changes to the database
            await m_UnturnedLogRepository.AddPlayerEventAsync(Eventdisconnected);
        }


        //  On Player Death also adds kills 
        public async Task HandleEventAsync(object sender, UnturnedPlayerDeathEvent @event)
        {

            var playerwhodied = @event.Player; // gets the player who died to update there information 

            // Gets current Player
            var player = await m_UnturnedLogRepository.FindPlayerAsync(playerwhodied.SteamId.ToString(), UserSearchMode.FindById);

            // Gets current server
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                         await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();


            // Constructs EventData object for the death
            var EventDataDeath = EventDatabase.BuildEventData(player, "Death", "Reason: " + @event.DeathCause, server);

            // Adds Event data above to the Database. 
            await m_UnturnedLogRepository.AddPlayerEventAsync(EventDataDeath);

            // Gets the instigator from the Event to work out if it was a person that killed them to add a kill to that person.
            var instigator = @event.Instigator;

            // Search the database using the instigator to see if its a valid user
            var instigatorplayer = await m_UnturnedLogRepository.FindPlayerAsync(instigator.ToString(), UserSearchMode.FindById);

            // If it doesnt return null we know it was a player who killed the person which triggered the event. 
            if (instigatorplayer != null) // If the event instigator for the Death event, is a player. 
            {

                // construct Event data for player kill 
                var EventDataKill = EventDatabase.BuildEventData(instigatorplayer, "Player Kill", "Player: " + instigatorplayer.Id, server);

                // Adds event data to the database
                await m_UnturnedLogRepository.AddPlayerEventAsync(EventDataKill);
            }

        }

        // On player CHat message.
        public async Task HandleEventAsync(object sender, UnturnedPlayerChattingEvent @event)
        {
            var eventplayer = @event.Player;

            // Gets current Player
            var player = await m_UnturnedLogRepository.FindPlayerAsync(eventplayer.SteamId.ToString(), UserSearchMode.FindById);

            // Gets current server
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                         await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            // construct Event data for chat message setting the content as the chat message itself
            var EventDataDeath = EventDatabase.BuildEventData(player, "Chat Message", "Content: " + @event.Message, server);

            // Adds event data to the database. 
            await m_UnturnedLogRepository.AddPlayerEventAsync(EventDataDeath);

        }

        // When a player teleports
        public async Task HandleEventAsync(object sender, UnturnedPlayerTeleportingEvent @event)
        {
            var eventplayer = @event.Player;

            // Gets current Player

            var player = await m_UnturnedLogRepository.FindPlayerAsync(eventplayer.SteamId.ToString(), UserSearchMode.FindById);
            
            // Gets the current server 
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                         await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            // Constructs Event Data 
            var EventDataDeath = EventDatabase.BuildEventData(player, "Teleported", " ", server);

            // Saves Event Data
            await m_UnturnedLogRepository.AddPlayerEventAsync(EventDataDeath);
        }


        // When a unturned player statistic increments.
        public async Task HandleEventAsync(object sender, UnturnedPlayerStatIncrementedEvent @event)
        {
            var player = @event.Player;

            var Event = @event.Stat;

            // Gets the player 
            var Eventplayer = await m_UnturnedLogRepository.FindPlayerAsync(player.SteamId.ToString(), UserSearchMode.FindById);

            // Gets the server
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                         await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            // If the stat incremented was Kills Animals ETC... construct a event Data for that then add it to the database. 
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
            else if (Event == EPlayerStat.KILLS_ZOMBIES_MEGA)
            {

                var PlayerEvent = EventDatabase.BuildEventData(Eventplayer, "Killed Mega Zombie", " ", server);
                await m_UnturnedLogRepository.AddPlayerEventAsync(PlayerEvent);
            }
            else if (Event == EPlayerStat.FOUND_RESOURCES)
            {

                var PlayerEvent = EventDatabase.BuildEventData(Eventplayer, "Found Resource", " ", server);
                await m_UnturnedLogRepository.AddPlayerEventAsync(PlayerEvent);
            }
            else if (Event == EPlayerStat.FOUND_PLANTS)
            {

                var PlayerEvent = EventDatabase.BuildEventData(Eventplayer, "Found Plants", " ", server);
                await m_UnturnedLogRepository.AddPlayerEventAsync(PlayerEvent);
            }
            else if (Event == EPlayerStat.FOUND_FISHES)
            {

                var PlayerEvent = EventDatabase.BuildEventData(Eventplayer, "Fish Caught", " ", server);
                await m_UnturnedLogRepository.AddPlayerEventAsync(PlayerEvent);
            }
            else if (Event == EPlayerStat.FOUND_BUILDABLES)
            {

                var PlayerEvent = EventDatabase.BuildEventData(Eventplayer, "Placed Buildable", " ", server);
                await m_UnturnedLogRepository.AddPlayerEventAsync(PlayerEvent);
            }
        }

        // When the player is banned
        public async Task HandleEventAsync(object sender, UnturnedPlayerBannedEvent @event)
        {
            var player = @event.BannedPlayer;

            // Gets current Player
            var pData = await m_UnturnedLogRepository.FindPlayerAsync(player.ToString(), UserSearchMode.FindById);

            // Gets the current server
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
                await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            // TODO: Need to fix the Reason and Instigator if executed by console. Check if Instigator = 0 and Reason = 0.
            var Eventpunnished = EventDatabase.BuildEventData(pData, "Player Banned", "Reason: " + @event.Reason + " Banned By: " + @event.Instigator, server);

            await m_UnturnedLogRepository.AddPlayerEventAsync(Eventpunnished);

        }



        // Gets player profile picture 
        private async Task<string> GetProfilePictureHashAsync(CSteamID user)
        {
            // Gets the Steam API Key from the configuration. 
            var apiKey = m_Configuration["steamWebApiKey"];

            // Check if its empty and if so return
            if (string.IsNullOrEmpty(apiKey))
                return "";

            // Creates new web client. 
            using var web = new WebClient();

            // Gets the string using the steam API where the id is the users steam api aswell as using the key 
            var result =
                await web.DownloadStringTaskAsync(
                    $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={apiKey}&steamids={user.m_SteamID}");

            // Deserializes the json return.
            var deserialized = JsonConvert.DeserializeObject<PlayerSummaries>(result);

            // Return the response making it into a avatar hash, which is a industry standard way of storing photo ids stored remoteley by steam. 
            return deserialized.response.players
                       .FirstOrDefault(k => k.steamid.Equals(user.ToString(), StringComparison.Ordinal))?.avatarhash ??
                   "";
        }


        // Gets players steam group
        private static async Task<string> GetSteamGroupNameAsync(CSteamID groupId)
        {
            // Creates a new web client 
            using var web = new WebClient();

            // Gets the String from a api using the Group id, returned in XML form. 
            var result =
                await web.DownloadStringTaskAsync("http://steamcommunity.com/gid/" + groupId +
                                                  "/memberslistxml?xml=1");
            // If There is no group name in the return 
            if (!result.Contains("<groupName>") || !result.Contains("</groupName>")) return "";

            // Sets the Start to the begining Group name tag which is the opening to get the name.
            var start = result.IndexOf("<groupName>", 0, StringComparison.Ordinal) + "<groupName>".Length;

            // Sets the end to the eng of the group name tag. 
            var end = result.IndexOf("</groupName>", start, StringComparison.Ordinal);

            // Gets the data by spliting the string between the start and the end. 
            var data = result.Substring(start, end - start);
            // Removes white space. 
            data = data.Trim();

            // Removes the brackests around it 
            data = data.Replace("<![CDATA[", "").Replace("]]>", "");

            // Returns Data.
            return data;
        }
    }

    // Class used for constructing objects
    class EventDatabase
    {
        public static PlayerData BuildPlayerData(ulong steamId, string characterName, string steamName, string hwid, uint ip,
            string profileHash, ulong questGroup, ulong steamGroup, string steamGroupName, double totalPlaytime, int Deaths,
            DateTime lastLogin, Server server)
        {
            // constructs the player data object using the data parsed into the function.
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
                ServerId = server.Id, 
                Server = server
            };
        }

        public static PlayerEvents BuildEventData(PlayerData player, string etype, string edata, Server server)
        {
            // constructs the plauer events object using the data parsed into the function.
            return new PlayerEvents
            {
                PlayerId = player.Id,
                EventType = etype,
                EventData = edata,
                ServerId = server.Id,
                Player = player,
                Server = server,
                EventTime = DateTime.Now
            };
        }
    }

    // Harmony patch class for some eventst hat are not avalible in the base api. Such as nodes cut down trees mind etc... 
    public class UnturnedPatches
    {

        public delegate void ResourceKill(CSteamID player, ResourceAsset asset);

        public static event ResourceKill? OnResourceKill;

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
                var playerId = __instance.channel.owner.playerID.steamID;

                OnResourceKill?.Invoke(playerId, asset);
            }
        }
    }
}