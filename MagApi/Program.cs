using MagApi.Identity;
using MagApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Mag application Starting Up");
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var identityDbContext = services.GetRequiredService<MagIdentityDbContext>();
                        var userManager = services.GetRequiredService<UserManager<MagApplicationUser>>();
                        var roleManager = services.GetRequiredService<RoleManager<MagApplicationRole>>();
                         //Migrate Identity Roles and Users
                        logger.Info("Migrate Identity Roles and Users");
                        identityDbContext.Database.Migrate();
            
                        //Seed Identity Roles and Users
                        logger.Info("Seed Identity Roles and Users");
                        await MagIdentityDbContextSeed.SeedEssentialsAsync(userManager, roleManager);

                        var dbContext = services.GetRequiredService<MagDbContext>();
                        //Migrate Warehouses and Carts
                        logger.Info("Migrate Warehouses and Carts");
                        dbContext.Database.Migrate();

                        //Seed Warehouses and Carts
                        logger.Info("Seed Warehouses and Carts");
                        await MagDbContextSeed.SeedEssentialsAsync(dbContext);
                        
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "An error occurred during migrate and seeding step.");
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                    {
                        configHost.SetBasePath(Directory.GetCurrentDirectory());
                    })
                .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseKestrel((context, serverOptions) =>
                        {
                            serverOptions.Configure(context.Configuration.GetSection("Kestrel"));
                        })
                        .UseStartup<Startup>();
                    })
                .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);                        
                    })
                .UseNLog();
    }
}
