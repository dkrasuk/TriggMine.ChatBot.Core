using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TriggMine.ChatBot.Repository.Context;
using TriggMine.ChatBot.Repository.Models;

namespace TriggMine.ChatBot.Repository.Repository
{
    public class ResolvedUrlRepository : IChatBotRepository<ResolvedUrl>
    {
        private readonly Func<IChatBotContext> _chatBotContext;

        public ResolvedUrlRepository(Func<IChatBotContext> chatBotContext)
        {
            _chatBotContext = chatBotContext ?? throw new ArgumentNullException(nameof(chatBotContext));
        }

        public async Task CreateOrUpdateAsync(ResolvedUrl value)
        {
            using (var db = _chatBotContext())
            {
                db.ResolvedUrls.Add(value);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteRecord(int id)
        {
            using (var db = _chatBotContext())
            {
                db.ResolvedUrls.Remove(db.ResolvedUrls.Find(id));
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ResolvedUrl>> GetAsyncList(Expression<Func<ResolvedUrl, bool>> predicate)
        {
            using (var db = _chatBotContext())
            {
                return await db.ResolvedUrls.Where(predicate).ToListAsync();
            }
        }

        public Task<ResolvedUrl> GetAsync(Expression<Func<ResolvedUrl, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task ModifyRecord(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
