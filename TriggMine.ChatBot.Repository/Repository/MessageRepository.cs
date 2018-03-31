using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TriggMine.ChatBot.Repository.Context;
using TriggMine.ChatBot.Repository.Models;

namespace TriggMine.ChatBot.Repository.Repository
{
    public class MessageRepository : IChatBotRepository<Message>
    {
        private readonly IChatBotContext _chatBotContext;

        public MessageRepository(IChatBotContext chatBotContext)
        {
            _chatBotContext = chatBotContext ?? throw new ArgumentNullException(nameof(chatBotContext));
        }

        public async Task CreateOrUpdateAsync(Message value)
        {
            using (var db = _chatBotContext)
            {
                if (db.Users.Any(n => n.UserId == value.UserId))
                {
                    db.Messages.Add(value);
                }
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Message>> GetAsync()
        {
            using (var db = _chatBotContext)
            {
                var messages = await db.Messages.ToListAsync();
                return messages;
            }
        }

        public Task<Message> GetAsync(Expression<Func<Message, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
