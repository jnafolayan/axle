using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axle.Engine;
using Axle.Engine.Crons;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Axle
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000");
                });
            });

            services.Configure<SearchEngineConfig>(
        Configuration.GetSection(nameof(SearchEngineConfig)));

            services.AddControllers();

            services.AddSingleton<SearchEngineConfig>(sp =>
            sp.GetRequiredService<IOptions<SearchEngineConfig>>().Value);

            services.AddSingleton<SearchEngine>();

            services.AddCronJob<IndexingCronJob>(config =>
            {
                config.TimeZoneInfo = TimeZoneInfo.Utc;
                config.CronExpression = @"0 */1 * * *";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
