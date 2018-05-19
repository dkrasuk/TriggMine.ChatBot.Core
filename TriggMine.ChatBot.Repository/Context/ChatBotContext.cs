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

        //public ChatBotContext(DbContextOptions<ChatBotContext> dbContextOptions)
        //    : base(dbContextOptions)
        //{
        //    Database.EnsureCreated();
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("User ID=dev_dbo;Password=dev_dbo;server=192.168.1.172;Port=5432;Database=TelegramBot;Pooling=true;",
                  x => x.MigrationsHistoryTable("__EFMigrationsHistory", "bot")
                 ).EnableSensitiveDataLogging();

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("bot");
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new ResolvedUrlConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<ResolvedUrl> ResolvedUrls { get; set; }
    }
}
