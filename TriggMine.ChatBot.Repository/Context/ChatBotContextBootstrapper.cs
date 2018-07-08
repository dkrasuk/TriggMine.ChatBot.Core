using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace TriggMine.ChatBot.Repository.Context
{
    public class ChatBotContextBootstrapper 
    {
        private ChatBotContextBootstrapper() { }
        public static ChatBotContextBootstrapper Instance { get; } = new ChatBotContextBootstrapper();
        public IConfiguration Configuration { get; set; }

        public string ConnectionString() => Configuration["ConnectionStrings:DefaultConnection"];
        public string Schema() => Configuration["ConnectionStrings:DefaultSchemaName"];
    }

}
