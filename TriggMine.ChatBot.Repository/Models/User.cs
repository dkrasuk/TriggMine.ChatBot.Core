using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TriggMine.ChatBot.Repository.Models
{
    public class User
    {        
        public int UserId { get; set; }       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public bool? IsBot { get; set; } = false;
        public string LanguageCode { get; set; }
        public DateTime DateFirstActivity { get; set; }
        public bool? IsBlocked { get; set; } = false;
        public ICollection<Message> Messages { get; set; }
    }
}
