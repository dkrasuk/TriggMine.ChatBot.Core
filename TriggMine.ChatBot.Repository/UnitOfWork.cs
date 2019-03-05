using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TriggMine.ChatBot.Repository.Context;
using TriggMine.ChatBot.Repository.Interfaces;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Repository.Repository;

namespace TriggMine.ChatBot.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatBotContext _dbContext;

        public UnitOfWork()
        {
            _dbContext = new ChatBotContext();
        }
        public void Dispose()
        {
            DisposeInternal(true);
        }

        private void DisposeInternal(bool disposing)
        {
            if (disposing)
            {
                _dbContext?.Dispose();
            }
        }
        public IRepository<User> UserRepository => new BaseRepository<User>(_dbContext);
        public IRepository<Message> MessageRepository => new BaseRepository<Message>(_dbContext);
        public IRepository<ResolvedUrl> ResolvedUrlRepository => new BaseRepository<ResolvedUrl>(_dbContext);
        public Task SaveChangeAsync() => _dbContext.SaveChangesAsync();
    }
}
