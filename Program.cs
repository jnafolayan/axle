using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axle.Server.Database;
using Axle.Server.Database.Models.Index;
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
            //CreateHostBuilder(args).Build().Run();


            // Create MongoDB instance and database to store Index 
            MongoCRUD db = new MongoCRUD("Index");

            //db.InsertRecord("Document");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
