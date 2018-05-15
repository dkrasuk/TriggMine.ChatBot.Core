using System;
using System.Collections.Generic;
using System.Text;

namespace TriggMine.ChatBot.Repository.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public int SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public DateTime DateSubscription { get; set; }
    }
}
