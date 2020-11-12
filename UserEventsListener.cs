// ReSharper disable AnnotateNotNullParameter
// ReSharper disable AnnotateNotNullTypeMember

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Players.Connections.Events;
using Edbtvplays.UnturnedLog.Unturned.API.Classes;
using Edbtvplays.UnturnedLog.Unturned.API.Classes.SteamWebApiClasses;
using Edbtvplays.UnturnedLog.Unturned.API.Services;
using Steamworks;

namespace Edbtvplays.UnturnedLog
{
    public class UserEventsListener : IEventListener<UnturnedPlayerConnectedEvent>, IEventListener<UnturnedPlayerDisconnectedEvent>
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


        //public async Task HandleEventAsync(object sender,  @event)
        //{
        //    var player = @event.Player;
        //    var playerId = player.SteamPlayer.playerID;
        //    var steamId = player.SteamId;
        //    var pfpHash = await GetProfilePictureHashAsync(steamId);
        //    var groupName = await GetSteamGroupNameAsync(playerId.group);
        //    var hwid = string.Join("", playerId.hwid);
        //    if (!player.SteamPlayer.transportConnection.TryGetIPv4Address(out var ip))
        //        ip = uint.MinValue;

        //    var pData = await m_UnturnedLogRepository.FindPlayerAsync(player.SteamId.ToString(), UserSearchMode.FindById);
        //    var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
        //                 await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

        //    if (pData == null)
        //    {
        //        pData = BuildPlayerData(steamId.m_SteamID, player.SteamPlayer.playerID.characterName,
        //            playerId.playerName, hwid, ip,
        //            pfpHash, player.Player.quests.groupID.m_SteamID, playerId.group.m_SteamID, groupName, 0,
        //            DateTime.Now, server);

        //        await m_UnturnedLogRepository.AddPlayerDataAsync(pData);
        //    }
        //    else
        //    {
        //        pData.ProfilePictureHash = pfpHash;
        //        pData.LastQuestGroupId = player.Player.quests.groupID.m_SteamID;
        //        pData.SteamGroup = playerId.group.m_SteamID;
        //        pData.SteamGroupName = groupName;
        //        pData.SteamName = playerId.playerName;
        //        pData.TotalPlaytime += DateTime.Now.Subtract(pData.LastLoginGlobal).TotalSeconds;

        //        await m_UnturnedLogRepository.SaveChangesAsync();
        //    }
        //}


        // Construct Player data returns a array of data which can then be sent to the database handler. 
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
                ServerId = server.Id,
                Server = server
            };
        }


        [ItemNotNull]

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

        [ItemNotNull]

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