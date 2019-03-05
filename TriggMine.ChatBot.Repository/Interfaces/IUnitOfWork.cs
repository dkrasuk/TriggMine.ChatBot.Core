using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TriggMine.ChatBot.Repository.Models;

namespace TriggMine.ChatBot.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Message> MessageRepository { get; }
        IRepository<ResolvedUrl> ResolvedUrlRepository { get; }
        Task SaveChangeAsync();
    }
}
