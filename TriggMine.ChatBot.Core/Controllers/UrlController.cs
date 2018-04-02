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
    [Route("api/Url")]
    public class UrlController : Controller
    {
        private readonly IResolverUrlService _resolverUrlService;

        public UrlController(IResolverUrlService resolverUrlService)
        {
            _resolverUrlService = resolverUrlService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _resolverUrlService.GetResolvedUrlsListAsync());
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]ResolvedUrlDTO resolvedUrlDto)
        {
            await _resolverUrlService.AddResolvedUrl(resolvedUrlDto);
            return Ok();
        }

        [HttpDelete("")]
        public async Task<IActionResult> Delete(int resolvedUrlId)
        {
            await _resolverUrlService.DeleteResolvedUrl(resolvedUrlId);
            return Ok();
        }

    }
}