using System;
using System.Collections.Generic;
using System.Text;

namespace TriggMine.ChatBot.Shared.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public long ChatId { get; set; }     
        public string Text { get; set; }
        public DateTime SendMessageDate { get; set; }
    }
}
