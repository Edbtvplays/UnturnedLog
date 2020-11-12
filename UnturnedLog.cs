using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;
using Edbtvplays.UnturnedLog.Unturned.Database;
using SDG.Unturned;
using Edbtvplays.UnturnedLog.Unturned.API.Classes;
using Edbtvplays.UnturnedLog.Unturned.API.Services;

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
        private readonly UnturnedLogStaticDbContext m_DbContext;
        private readonly IUnturnedLogRepository m_playerinfolib;

        public UnturnedLog(
            IConfiguration configuration, 
            IStringLocalizer stringLocalizer,
            ILogger<UnturnedLog> logger, 
            UnturnedLogStaticDbContext dbcontext,
            IUnturnedLogRepository playerinforepository,
            IServiceProvider serviceProvider) : base(serviceProvider)
            
        {
            m_Configuration = configuration;
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
            m_DbContext = dbcontext;
            m_playerinfolib = playerinforepository;
        }

        protected override async UniTask OnLoadAsync()
        {
			await UniTask.SwitchToMainThread(); 
            m_Logger.LogInformation("UnturnedLog for Unturned by Edbtvplays was loaded correctly");

            checkTPS().Forget();
			
			await UniTask.SwitchToThreadPool(); 
        }

        protected override async UniTask OnUnloadAsync()
        {
            m_Logger.LogInformation("UnturnedLog for Unturned by Edbtvplays was unloaded correctly");

            await UniTask.SwitchToMainThread();
        }

        private async UniTask checkTPS()
        {
            while (IsComponentAlive)
            {
                var server = await m_playerinfolib.GetCurrentServerAsync() ??
                             await m_playerinfolib.CheckAndRegisterCurrentServerAsync();

                int tps = Provider.debugTPS;
                m_DbContext.TPS.Add(new TPS
                {
                    Value = tps,
                    ServerId = server.Id,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

                await m_DbContext.SaveChangesAsync();
                await UniTask.Delay(2000);
            }
        }
    }
}
