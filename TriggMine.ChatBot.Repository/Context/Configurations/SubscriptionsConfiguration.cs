using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TriggMine.ChatBot.Repository.Models;

namespace TriggMine.ChatBot.Repository.Context.Configurations
{
    public class SubscriptionsConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTable("subscriptions");
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.ChatId);
            builder.HasIndex(c => c.SubscriptionId);

            builder.Property(c => c.Id)
                .HasColumnName("id");

            builder.Property(c => c.ChatId)
                .HasColumnName("chatId");

            builder.Property(c => c.SubscriptionId)
                .HasColumnName("subscriptionId");

            builder.Property(c => c.SubscriptionName)
                .HasColumnName("subscriptionName");

            builder.Property(s => s.DateSubscription)
                .HasColumnName("dateSubscription")
                .HasDefaultValueSql("(now() at time zone 'utc')")
                .ValueGeneratedOnAdd();
        }
    }
}
