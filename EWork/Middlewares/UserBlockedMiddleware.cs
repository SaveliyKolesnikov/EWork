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
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserBlockedMiddleware(RequestDelegate next, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _next = next;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task Invoke(HttpContext context)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (!(user is null) && user.IsBlocked)
            {
                await _signInManager.SignOutAsync();
                context.Response.Redirect("/Identity/Account/Login");
            }

            if (!(_next is null))
                await _next.Invoke(context);
        }
    }
}
