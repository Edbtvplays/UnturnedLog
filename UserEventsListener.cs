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
using OpenMod.Unturned.Players.Connections.Events;
using OpenMod.Unturned.Players.Life.Events;
using OpenMod.Unturned.Players.Stats.Events;
using OpenMod.Unturned.Players.Chat.Events;
using OpenMod.Unturned.Players.Bans.Events;
using Edbtvplays.UnturnedLog.Unturned.API.Classes;
using Edbtvplays.UnturnedLog.Unturned.API.Classes.SteamWebApiClasses;
using Edbtvplays.UnturnedLog.Unturned.API.Services;
using Steamworks;

namespace Edbtvplays.UnturnedLog
{
    public class UserEventsListener : IEventListener<UnturnedPlayerConnectedEvent>, IEventListener<UnturnedPlayerDisconnectedEvent>, IEventListener<UnturnedPlayerDeathEvent>, IEventListener<UnturnedPlayerChattingEvent>, IEventListener<UnturnedPlayerBannedEvent> //, IEventListener<UnturnedPlayerStatIncrementedEvent>
    {
        private readonly IUnturnedLogRepository m_UnturnedLogRepository;
        private readonly IConfiguration m_Configuration;

        public UserEventsListener(IUnturnedLogRepository UnturnedLogRepository, IConfiguration configuration)
        {
            m_UnturnedLogRepository = UnturnedLogRepository;
            m_Configuration = configuration;
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
                pData = BuildPlayerData(steamId.m_SteamID, player.SteamPlayer.playerID.characterName,
                    playerId.playerName, hwid, ip,
                    pfpHash, questGroupId, playerId.group.m_SteamID, groupName, 0,
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
                pData = BuildPlayerData(steamId.m_SteamID, player.SteamPlayer.playerID.characterName,
                    playerId.playerName, hwid, ip,
                    pfpHash, player.Player.quests.groupID.m_SteamID, playerId.group.m_SteamID, groupName, 0,
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
        }

        //  On Player Death also adds kills (temporary untill OM Updates) 
        public async Task HandleEventAsync(object sender, UnturnedPlayerDeathEvent @event)
        {
            var playerwhodied = @event.Player; // gets the player who died to update there information 

            // Gets current Player
            var pDatadied = await m_UnturnedLogRepository.FindPlayerAsync(playerwhodied.SteamId.ToString(), UserSearchMode.FindById);

            var DeathCount = pDatadied.Deaths + 1;

            // Set the stuff to be saved into the pdata object to be saved into the database Only stuff that is modified here needs to be set

            pDatadied.Deaths = DeathCount; 

            await m_UnturnedLogRepository.SaveChangesAsync(); // Save changes to the database for the player who died.

            if (@event.Instigator != null ) // If the event instigator for the Death event, is a player. 
            {
                var playerwhokilled = @event.Instigator; // Update the player who killed the player who died to update there playerkills. 

                var pDatakilled = await m_UnturnedLogRepository.FindPlayerAsync(playerwhokilled.ToString(), UserSearchMode.FindById);

                var Killcount = pDatakilled.PlayerKills + 1;
            }
        }

        // On player Headshot and death by zombie 
        //public async Task HandleEventAsync(object sender, PlayerLife.onDamaged @event)
        //{
        //    var player = @event.Player;

        //    var pData = await m_UnturnedLogRepository.FindPlayerAsync(player.SteamId.ToString(), UserSearchMode.FindById);

        //    // Need to figure out a way to detect if a player is killed. This is very Much TODO.  
        //    if (@event.Limb = Elimb.SKULL) // Check if the damage event is for the head  to update the headshot value. 
        //    {
        //        var Headshot = pData.PlayerKills + 1;
        //        pData.Headshots = Headshot;

        //        await m_UnturnedLogRepository.SaveChangesAsync(); // Save changes to the database.

        //    }
        //}

        // On player CHat message.
        public async Task HandleEventAsync(object sender, UnturnedPlayerChattingEvent @event)
        {
            var player = @event.Player;

            // Gets current Player
            var pData = await m_UnturnedLogRepository.FindPlayerAsync(player.SteamId.ToString(), UserSearchMode.FindById);

            var ChatMessages = pData.TotalChatMessages + 1;

            // Set the stuff to be saved into the pdata object to be saved into the database Only stuff that is modified here needs to be set

            pData.TotalChatMessages = ChatMessages;

            await m_UnturnedLogRepository.SaveChangesAsync(); // Save changes to the database.
        }


        // Here i need to add UnturnedplayerstatIncrementedevent, i am waiting for a update to the API which hooks into the event to do this.


        //public async Task HandleEventAsync(object sender, UnturnedPlayerStatIncrementedEvent @event)
        //{
        //    var player = @event.Player;

        //    var Event = @event.Stat;


        //    if (Event = KILLS_PLAYERS)
        //    {



        //    }


        //    var player = @event.Player;

        //    // Gets current Player
        //    var pData = await m_UnturnedLogRepository.FindPlayerAsync(player.SteamId.ToString(), UserSearchMode.FindById);

        //    var ChatMessages = pData.TotalChatMessages + 1;

        //    // Set the stuff to be saved into the pdata object to be saved into the database Only stuff that is modified here needs to be set

        //    pData.TotalChatMessages = ChatMessages;

        //    await m_UnturnedLogRepository.SaveChangesAsync(); // Save changes to the database.
        //}

        public async Task HandleEventAsync(object sender, UnturnedPlayerBannedEvent @event)
        {
            var player = @event.BannedPlayer;

            // Gets current Player
            var pData = await m_UnturnedLogRepository.FindPlayerAsync(player.ToString(), UserSearchMode.FindById);

            var Punishments = pData.Punishments + 1;

            // Set the stuff to be saved into the pdata object to be saved into the database Only stuff that is modified here needs to be set

            pData.Punishments = Punishments;

            await m_UnturnedLogRepository.SaveChangesAsync(); // Save changes to the database.
        }


        // Construct Player data returns a object of data which can then be sent to the database handler. This is for new data only.
        private static PlayerData BuildPlayerData(ulong steamId, string characterName, string steamName, string hwid, uint ip,
            string profileHash, ulong questGroup, ulong steamGroup, string steamGroupName, double totalPlaytime, 
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
    }
}