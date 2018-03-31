using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Shared.Helpers;

namespace TriggMine.ChatBot.Repository.Context.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user");

            builder.HasKey(s => s.UserId);
            builder.HasIndex(s => s.UserId);

            builder.Property(s => s.UserId)
                .HasColumnName("userId")
                .IsRequired();
           
            builder.Property(s => s.FirstName)
                .HasColumnName("firsName");

            builder.Property(s => s.LastName)
                .HasColumnName("lastName");

            builder.Property(s => s.Username)
                .HasColumnName("userName");

            builder.Property(s => s.IsBot)
                .HasColumnName("isBot");

            builder.Property(s => s.LanguageCode)
                .HasColumnName("languageCode");

            builder.Property(s => s.DateFirstActivity)
                .HasColumnName("dateFirstActivity")
                .HasDefaultValueSql("(now() at time zone 'utc')")
                .ValueGeneratedOnAdd();

            builder.Property(s => s.IsBlocked)
                .HasColumnName("isBlocked");

            builder.HasMany(p => p.Messages)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .HasPrincipalKey(p => p.UserId);
                
                
        }
    }
}
