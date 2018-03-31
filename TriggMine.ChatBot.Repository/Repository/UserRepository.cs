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
    public class UserRepository : IChatBotRepository<User>
    {
        private readonly IChatBotContext _chatBotContext;

        public UserRepository(IChatBotContext chatBotContext)
        {
            _chatBotContext = chatBotContext ?? throw new ArgumentException(nameof(chatBotContext));
        }
        public async Task CreateOrUpdateAsync(User value)
        {
            using (var db = _chatBotContext)
            {
                try
                {
                    if (!db.Users.Any(n => n.UserId == value.UserId))
                    {
                        db.Users.Add(value);
                    }
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);                    
                }
                
            }
        }


        public async Task<IEnumerable<User>> GetAsync()
        {
            using (var db = _chatBotContext)
            {
                var users = await db.Users.Include(p => p.Messages).ToArrayAsync();
                return users;
            }
        }

        public Task<User> GetAsync(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
