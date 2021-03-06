﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace NaniWeb
{
    public static class Program
    {
        public static async Task Main(string[] args) => await Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>()).Build().RunAsync();
    }
}