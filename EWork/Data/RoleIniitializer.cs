using System.Threading.Tasks;
using EWork.Models;
using Microsoft.AspNetCore.Identity;

namespace EWork.Data
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            const string moderatorEmail = "moderator@gmail.com";
            const string username = "moderator";
            const string password = "asdzxc123456";

            await AddRoleAsync(roleManager, "moderator");
            await AddRoleAsync(roleManager, "emoloyer");
            await AddRoleAsync(roleManager, "freelancer");

            if (await userManager.FindByNameAsync(username) is null)
            {
                var moderator = new Moderator
                {
                    Name = "Moderator",
                    Surname = "Moderator",
                    Email = moderatorEmail,
                    UserName = username
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