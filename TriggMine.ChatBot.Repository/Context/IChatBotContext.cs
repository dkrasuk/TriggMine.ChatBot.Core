using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TriggMine.ChatBot.Repository.Models;

namespace TriggMine.ChatBot.Repository.Context
{
    public interface IChatBotContext : IDisposable
    {
        DbSet<User> Users { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<ResolvedUrl> ResolvedUrls { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
