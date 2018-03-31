using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TriggMine.ChatBot.Core.Services;
using TriggMine.ChatBot.Shared.DTO;

namespace TriggMine.ChatBot.Core.Controllers
{
    [Produces("application/json")]
    [Route("api/message")]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetMessages()
        {
            return Ok(await _messageService.GetMessageAsync());
        }

        [HttpPost("")]
        public async Task<IActionResult> PostMessage([FromBody] MessageDTO message)
        {
            await _messageService.CreateMessage(message);
            return Ok();
        }
    }
}