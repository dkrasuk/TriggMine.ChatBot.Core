using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TriggMine.ChatBot.Repository.Repository
{
    public interface IChatBotRepository<T>
    {
        Task<IEnumerable<T>> GetAsyncList(Expression<Func<T, bool>> predicate);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        Task CreateOrUpdateAsync(T value);
        Task ModifyRecord(int userId);
    }
}
