using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TriggMine.ChatBot.Core.Services.Interfaces;

namespace TriggMine.ChatBot.Core.Controllers.MVC
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        public HomeController(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var a = await _userService.GetAllUser(c => true);
            return View(a);
        }
    }
}