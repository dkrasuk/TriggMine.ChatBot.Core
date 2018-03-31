using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TriggMine.ChatBot.Repository.Context.Configurations;
using TriggMine.ChatBot.Repository.Models;

namespace TriggMine.ChatBot.Repository.Context
{
    public class ChatBotContext : DbContext, IChatBotContext
    {
        public ChatBotContext(DbContextOptions<ChatBotContext> dbContextOptions)
            : base(dbContextOptions)
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ChatBot");
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
    }
}
