using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Data;
using EWork.Exceptions;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EWork
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var photoOptions = services.GetRequiredService<IOptions<PhotoConfig>>();
                    var usersOptions = services.GetRequiredService<IOptions<UsersConfig>>();
                    await RoleInitializer.InitializeAsync(userManager, rolesManager, photoOptions, usersOptions);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }

                try
                {
                    var repository = services.GetRequiredService<IBalanceManager>();
                    var freelancingPlatformOptions = services.GetRequiredService<IOptions<FreelancingPlatformConfig>>();
                    var balanceChecker = new FreelancingPlatformBalanceChecker(repository, freelancingPlatformOptions);
                    await balanceChecker.CheckAsync();
                }
                catch (DbNotInitializedBalanceException e)
                {
                    Console.WriteLine(e);
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, e.Message);
                    ExceptionDispatchInfo.Capture(e).Throw();
                }
            }

            await host.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
