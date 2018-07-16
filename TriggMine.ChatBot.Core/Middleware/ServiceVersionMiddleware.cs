using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriggMine.ChatBot.Core.Middleware
{
    public class ServiceVersionMiddleware
    {
        private readonly RequestDelegate _next;

        public ServiceVersionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.ToLower() == "/api/serviceversion" && context.Request.Method.ToLower() == "get")
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("version 2.0.2");
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
