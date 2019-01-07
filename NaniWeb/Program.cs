using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NaniWeb.Others;

namespace NaniWeb
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().ConfigureKestrel((context, options) =>
            {
                options.Limits.MaxRequestBodySize = 100000000;
                
                if (context.HostingEnvironment.IsProduction())
                {
                    options.Listen(IPAddress.Any, 80, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
                    options.Listen(IPAddress.Any, 443, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                        listenOptions.UseHttps(Utils.GetCertificate());
                    });
                }
            });
        }
    }
}