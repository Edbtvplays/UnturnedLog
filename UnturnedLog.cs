using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.Extensions;
using Edbtvplays.UnturnedLog.Unturned.Database;
using SDG.Unturned;
using Edbtvplays.UnturnedLog.Unturned.API.Classes;
using Edbtvplays.UnturnedLog.Unturned.API.Services;
using Fleck;
using System.Collections.Generic;
using Edbtvplays.UnturnedLog;
using OpenMod.API.Users;
using Steamworks;

[assembly: PluginMetadata("Edbtvplays.UnturnedLog", Author = "Edbtvplays",
    DisplayName = "Unturned Log",
    Website = "https://edbrook.site")]


namespace UnturnedLog
{
    public class UnturnedLog : OpenModUnturnedPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<UnturnedLog> m_Logger;
        private readonly UnturnedLogDbContext m_DbContext;
        private readonly IUnturnedLogRepository m_UnturnedLogRepository;


        public UnturnedLog(IConfiguration configuration, IStringLocalizer stringLocalizer, ILogger<UnturnedLog> logger, UnturnedLogDbContext dbcontext, IUnturnedLogRepository unturnedLogRepository,
            IServiceProvider serviceProvider) : base(serviceProvider)

        {
            m_Configuration = configuration;
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
            m_DbContext = dbcontext;
            m_UnturnedLogRepository = unturnedLogRepository;
        }

        protected override async UniTask OnLoadAsync()
        {
            await UniTask.SwitchToMainThread();
            m_Logger.LogInformation("UnturnedLog for Unturned by Edbtvplays was loaded correctly");

            await m_DbContext.OpenModMigrateAsync();

            await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            Subscribe();

            await UniTask.SwitchToThreadPool();
        }

        protected override UniTask OnUnloadAsync()
        {
            UnSubscribe();

            m_Logger.LogInformation("UnturnedLog for Unturned by Edbtvplays was unloaded correctly");

           

            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            UnturnedPatches.OnResourceKill += Events_OnResourceKill;
        }

        // Unsubscribes to the Event
        public void UnSubscribe()
        {
            UnturnedPatches.OnResourceKill -= Events_OnResourceKill;
        }

        public async void Events_OnResourceKill(CSteamID player, ResourceAsset asset)
        {
            var instigatorplayer = await m_UnturnedLogRepository.FindPlayerAsync(player.ToString(), UserSearchMode.FindById);
            var server = await m_UnturnedLogRepository.GetCurrentServerAsync() ??
             await m_UnturnedLogRepository.CheckAndRegisterCurrentServerAsync();

            var EventTreeCut = EventDatabase.BuildEventData(instigatorplayer, "Resource harvested", " ", server);

            await m_UnturnedLogRepository.AddPlayerEventAsync(EventTreeCut);
        }



        // Previous abandoned implementation for adding to a database, found to be to cluncky and way to compliments. 

        //private async UniTask checkTPS()
        //{
        //    while (IsComponentAlive)
        //    {
        //        var server = await m_playerinfolib.GetCurrentServerAsync() ??
        //                     await m_playerinfolib.CheckAndRegisterCurrentServerAsync();

        //        int tps = Provider.debugTPS;
        //        m_DbContext.TPS.Add(new TPS
        //        {
        //            Value = tps,
        //            ServerId = server.Id,
        //            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        //        });

        //        await m_DbContext.SaveChangesAsync();
        //        await UniTask.Delay(2000);
        //    }
        //}
    }
}
