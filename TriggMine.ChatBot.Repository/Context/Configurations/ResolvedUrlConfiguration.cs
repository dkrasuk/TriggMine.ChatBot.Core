using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TriggMine.ChatBot.Repository.Models;

namespace TriggMine.ChatBot.Repository.Context.Configurations
{
    public class ResolvedUrlConfiguration : IEntityTypeConfiguration<ResolvedUrl>
    {
        public void Configure(EntityTypeBuilder<ResolvedUrl> builder)
        {
            builder.ToTable("resolved_Url");

            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("id");

            builder.Property(c => c.Url)
                .HasColumnName("url");
        }
    }
}
