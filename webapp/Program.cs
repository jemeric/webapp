using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using webapp.Util;

namespace webapp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IWebHost webHost = BuildWebHost(args)
                .UseWebRoot(AppConstants.webRoot)
                .Build();
            /* perform initialization here - see https://www.thomaslevesque.com/2018/09/25/asynchronous-initialization-in-asp-net-core-revisited/ */
            await webHost.InitAsync();
            webHost.Run();
        }

        public static IWebHostBuilder BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
