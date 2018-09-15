using System;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace EWork.Data
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            IOptions<PhotoConfig> photoOptions, IOptions<UsersConfig> usersOptions)
        {
            await AddRoleAsync(roleManager, "administrator");
            await AddRoleAsync(roleManager, "moderator");
            await AddRoleAsync(roleManager, "employer");
            await AddRoleAsync(roleManager, "freelancer");

            foreach (var moderatorData in usersOptions.Value.Moderators)
            {
                var moderator = new Moderator
                {
                    Name = moderatorData.Name,
                    Surname = moderatorData.Surname,
                    Email = moderatorData.Email,
                    UserName = moderatorData.UserName,
                    SignUpDate = DateTime.Now,
                    Balance = new Balance(),
                    ProfilePhotoName = photoOptions.Value.DefaultPhoto
                };

                await AddUserAsync(moderator, moderatorData.Password, moderator.Role);
            }

            foreach (var administratorData in usersOptions.Value.Administrators)
            {
                var administrator = new Administrator
                {
                    Name = administratorData.Name,
                    Surname = administratorData.Surname,
                    Email = administratorData.Email,
                    UserName = administratorData.UserName,
                    SignUpDate = DateTime.Now,
                    Balance = new Balance(),
                    ProfilePhotoName = photoOptions.Value.DefaultPhoto
                };

                await AddUserAsync(administrator, administratorData.Password, administrator.Role);
            }
            
            async Task AddUserAsync(User user, string password, string role)
            {
                if (await userManager.FindByNameAsync(user.UserName) is null)
                {
                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
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