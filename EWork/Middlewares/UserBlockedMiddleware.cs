using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EWork.Middlewares
{
    public class UserBlockedMiddleware
    {
        private readonly RequestDelegate _next;

        public UserBlockedMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            var userManager = context.RequestServices.GetRequiredService<UserManager<User>>();
            var signInManager = context.RequestServices.GetRequiredService<SignInManager<User>>();
            var user = await userManager.GetUserAsync(context.User);
            if (!(user is null) && user.IsBlocked)
            {
                await signInManager.SignOutAsync();
                context.Response.Redirect("/Identity/Account/Login");
            }

            if (!(_next is null))
                await _next.Invoke(context);
        }
    }
}
