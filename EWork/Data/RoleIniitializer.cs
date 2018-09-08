using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace EWork.Data
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<PhotoConfig> photoOptions)
        {
            const string moderatorEmail = "moderator@gmail.com";
            const string username = "moderator";
            const string password = "asdzxc";

            await AddRoleAsync(roleManager, "moderator");
            await AddRoleAsync(roleManager, "employer");
            await AddRoleAsync(roleManager, "freelancer");

            if (await userManager.FindByNameAsync(username) is null)
            {
                var moderator = new Moderator
                {
                    Name = "Moderator",
                    Surname = "Moderator",
                    Email = moderatorEmail,
                    UserName = username,
                    SingUpDate = DateTime.Now,
                    Balance = new Balance(),
                    Jobs = new List<Job>(),
                    Notifications = new List<Notification>(),
                    ProfilePhotoName = photoOptions.Value.DefaultPhoto
                };

                var result = await userManager.CreateAsync(moderator, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(moderator, "moderator");
                }
            }
        }

        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}