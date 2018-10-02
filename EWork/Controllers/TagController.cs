using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EWork.Controllers
{
    [Authorize]
    public class TagController : Controller
    {
        private readonly ITagManager _tagManager;
        private readonly int _takeAmount;

        public TagController(ITagManager tagManager, IOptions<FreelancingPlatformConfig> options)
        {
            _tagManager = tagManager;
            _takeAmount = options.Value.TakeAmount;
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<JsonResult> GetTagsByFirstLetters(string tagVal)
        {
            var result = await _tagManager.FindByFirstLetters(tagVal).Take(_takeAmount).Select(tag => tag.Text).ToArrayAsync();
            return Json(result);
        }
    }
}