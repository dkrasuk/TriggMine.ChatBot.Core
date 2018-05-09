using System;
using System.Collections.Generic;
using System.Text;

namespace TriggMine.ChatBot.Shared.DTO
{
    public class Translate
    {       
        public int Code { get; set; }
        public string Lang { get; set; }
        public string[] Text { get; set; }
    }
}
