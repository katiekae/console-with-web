using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace ConsoleWithWeb
{
    public class WebProgram
    {
        private readonly IHost _host;
        
        public WebProgram(string[] args, int listenPort)
        {
            _host = CreateHostBuilder(args, listenPort).Build();
            BridgeInstance = (BridgeService)_host.Services.GetService(typeof(BridgeService));
        }
        
        public BridgeService BridgeInstance { get; }
        
        public void RunSync()
        {
            _host.Run();
        }

        public void Shutdown()
        {
            _host.StopAsync().GetAwaiter().GetResult();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, int listenPort) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                        {
                            #region Configure Limits

                            serverOptions.Limits.MaxConcurrentConnections = 100;
                            serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
                            serverOptions.Limits.MaxRequestBodySize = 10 * 1024;
                            serverOptions.Limits.MinRequestBodyDataRate =
                                new MinDataRate(bytesPerSecond: 100,
                                    gracePeriod: TimeSpan.FromSeconds(10));
                            serverOptions.Limits.MinResponseDataRate =
                                new MinDataRate(bytesPerSecond: 100,
                                    gracePeriod: TimeSpan.FromSeconds(10));
                            serverOptions.Limits.KeepAliveTimeout =
                                TimeSpan.FromMinutes(2);
                            serverOptions.Limits.RequestHeadersTimeout =
                                TimeSpan.FromMinutes(1);

                            #endregion

                            serverOptions.Listen(IPAddress.Loopback, listenPort);
                            //                        serverOptions.Listen(IPAddress.Loopback, 5001, 
                            //                            listenOptions =>
                            //                            {
                            //                                listenOptions.UseHttps("testCert.pfx", 
                            //                                    "testPassword");
                            //                            });
                        })
                        .UseStartup<Startup>();
                });
    }
}