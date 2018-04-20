using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Shared.Helpers;

namespace TriggMine.ChatBot.Repository.Context.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("message");
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => s.ChatId);
            builder.HasIndex(s => s.MessageId);
            builder.HasIndex(s => s.Id);

            builder.Property(s => s.Id)
                .HasColumnName("id");

            builder.Property(s => s.MessageId)
                .HasColumnName("messageId");

            builder.Property(s => s.ChatId)
                .HasColumnName("chatId");

            builder.Property(s => s.ChatTitle)
                .HasColumnName("chatTitle");

            builder.Property(s => s.Text)
                .HasColumnName("text");

            builder.Property(s => s.SendMessageDate)
                .HasColumnName("sendMessageDate")
                .HasDefaultValueSql("(now() at time zone 'utc')")
                .ValueGeneratedOnAdd();

            builder.HasOne(p => p.User)
                .WithMany(p => p.Messages)
                .HasForeignKey(p => p.UserId)
                .HasPrincipalKey(s => s.UserId);               
        }
    }
}
