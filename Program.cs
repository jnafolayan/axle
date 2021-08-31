using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axle.Engine.Database;
using Axle.Engine.Database.Models.Index;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Axle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create database to store Index 
            MongoCRUD IndexCRUD = new MongoCRUD("Index");

            CreateHostBuilder(args).Build().Run();


        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
