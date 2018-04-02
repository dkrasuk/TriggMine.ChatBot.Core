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

        private readonly Func<IChatBotContext> _chatBotContext;

        public UserRepository(Func<IChatBotContext> chatBotContext)
        {
            _chatBotContext = chatBotContext ?? throw new ArgumentException(nameof(chatBotContext));
        }
        public async Task CreateOrUpdateAsync(User value)
        {
            using (var db = _chatBotContext())
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

        public async Task ModifyRecord(int userId)
        {
            using (var db = _chatBotContext())
            {
                if (db.Users.Any(n => n.UserId == userId))
                {
                    var user = db.Users.Find(userId);
                    user.IsBlocked = true;
                    user.DateBlockedUser = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                }
            }
        }        

        public async Task<User> GetAsync(Expression<Func<User, bool>> predicate)
        {
            using (var db = _chatBotContext())
            {
                return await db.Users.Where(predicate).FirstOrDefaultAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAsyncList(Expression<Func<User, bool>> predicate)
        {
            using (var db = _chatBotContext())
            {
                return (await db.Users.Where(predicate).ToListAsync());
            }
        }

        public Task DeleteRecord(int id)
        {
            throw new NotImplementedException();
        }
    }
}
