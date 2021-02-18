﻿using System;
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

        public WebSocketServer wServer;

        public List<IWebSocketConnection> wSockets = new List<IWebSocketConnection>();


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


            websocket();
			
			await UniTask.SwitchToThreadPool(); 
        }

        protected override UniTask OnUnloadAsync()
        {
            m_Logger.LogInformation("UnturnedLog for Unturned by Edbtvplays was unloaded correctly");

            return UniTask.CompletedTask;
        }


        public void websocket()
        {
            // Websocket Server on localhost port 8080.
            wServer = new WebSocketServer("ws://0.0.0.0:27081");

            wServer.Start(wSocket =>
            {
                wSocket.OnOpen = () => Console.WriteLine("Open!");

                wSocket.OnClose = () => Console.WriteLine("Close!");

                wSocket.OnMessage = message => HandleMessage(message);
            });
        }


        private void HandleMessage(string Message)
        {
            if (Message.ToLower() == "playercount") // Returns PlayerCount 
            {
                SendToClients(Provider.clients.Count.ToString());
            }

            // Need to return TPS. Aswell as playerCount. 
        }

        private void SendToClients(string Message)
        {
            wSockets.ForEach(wSocket => { wSocket.Send(Message); });
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
