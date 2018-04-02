using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TriggMine.ChatBot.Core.Services.Interfaces;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(bool? isBlocked)
        {
            return Ok(await _userService.GetAllUser(c => (isBlocked != null) ? c.IsBlocked == isBlocked : (c.IsBlocked == true || c.IsBlocked == false)));
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int userId)
        {
            return Ok(await _userService.FindUser(c => c.UserId == userId));
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody] UserDTO user)
        {
            await _userService.CreateUser(user);
            return Ok();
        }

    }
}