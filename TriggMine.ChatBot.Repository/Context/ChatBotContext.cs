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
            optionsBuilder.UseNpgsql("User ID=NOTIFICATION;Password=NOTIFICATION;server=postgresqldb.cdnzpuvcmatr.us-west-2.rds.amazonaws.com;Port=5432;Database=postgresql;Pooling=true;",
                  x => x.MigrationsHistoryTable("__EFMigrationsHistory", "ChatBot")
                 ).EnableSensitiveDataLogging();

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ChatBot");
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new ResolvedUrlConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionsConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<ResolvedUrl> ResolvedUrls { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
    }
}
