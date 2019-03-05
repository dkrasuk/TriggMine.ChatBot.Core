using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TriggMine.ChatBot.Repository.Context.Configurations;
using TriggMine.ChatBot.Repository.Models;

namespace TriggMine.ChatBot.Repository.Context
{
    public class ChatBotContext : DbContext
    {
        private string ConnectionString { get; set; }
        private string Schema { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConnectionString = ChatBotContextBootstrapper.Instance.ConnectionString();
            Schema = ChatBotContextBootstrapper.Instance.Schema();

            optionsBuilder.UseNpgsql(ConnectionString,
                  x => x.MigrationsHistoryTable("__EFMigrationsHistory", Schema)
                 ).EnableSensitiveDataLogging();

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);
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
