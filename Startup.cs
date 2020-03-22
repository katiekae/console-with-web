using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace ConsoleWithWeb
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddSingleton<BridgeService, BridgeService>();
        }
        
        public void Configure(IApplicationBuilder app)
        {
            var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                RequestPath = ""
            });

            app.UseMvc();

            app.Run(async (context) =>
            {
                #region Configure Limits

                context.Features.Get<IHttpMaxRequestBodySizeFeature>()
                        .MaxRequestBodySize = 10 * 1024;

                var minRequestRateFeature =
                    context.Features.Get<IHttpMinRequestBodyDataRateFeature>();
                var minResponseRateFeature =
                    context.Features.Get<IHttpMinResponseDataRateFeature>();

                if (minRequestRateFeature != null)
                {
                    minRequestRateFeature.MinDataRate = new MinDataRate(
                        bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                }

                if (minResponseRateFeature != null)
                {
                    minResponseRateFeature.MinDataRate = new MinDataRate(
                        bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                }

                #endregion

                context.Response.ContentType = "text/html";
                await context.Response
                    .WriteAsync("<!DOCTYPE html><html lang=\"en\"><head>" +
                                "<title></title></head><body><p>Hosted by Kestrel</p>");

                if (serverAddressesFeature != null)
                {
                    await context.Response
                        .WriteAsync("<p>Listening on the following addresses: " +
                                    string.Join(", ", serverAddressesFeature.Addresses) +
                                    "</p>");
                }

                await context.Response.WriteAsync("<p>Request URL: " +
                                                  $"{context.Request.GetDisplayUrl()}<p>");
            });
        }
    }
}