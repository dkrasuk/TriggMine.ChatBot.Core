﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using TriggMine.ChatBot.Repository.Context;

namespace TriggMine.ChatBot.Repository.Migrations
{
    [DbContext(typeof(ChatBotContext))]
    [Migration("20180510121816_AddCollumnTypeMessage")]
    partial class AddCollumnTypeMessage
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("ChatBot")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("TriggMine.ChatBot.Repository.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<long>("ChatId")
                        .HasColumnName("chatId");

                    b.Property<string>("ChatTitle")
                        .HasColumnName("chatTitle");

                    b.Property<int>("MessageId")
                        .HasColumnName("messageId");

                    b.Property<DateTime>("SendMessageDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("sendMessageDate")
                        .HasDefaultValueSql("(now() at time zone 'utc')");

                    b.Property<string>("Text")
                        .HasColumnName("text");

                    b.Property<string>("Type")
                        .HasColumnName("type");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("message");
                });

            modelBuilder.Entity("TriggMine.ChatBot.Repository.Models.ResolvedUrl", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Url")
                        .HasColumnName("url");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("resolved_Url");
                });

            modelBuilder.Entity("TriggMine.ChatBot.Repository.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("userId");

                    b.Property<DateTime?>("DateBlockedUser")
                        .HasColumnName("dateBlockedUser");

                    b.Property<DateTime>("DateFirstActivity")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("dateFirstActivity")
                        .HasDefaultValueSql("(now() at time zone 'utc')");

                    b.Property<string>("FirstName")
                        .HasColumnName("firsName");

                    b.Property<bool?>("IsBlocked")
                        .HasColumnName("isBlocked");

                    b.Property<bool?>("IsBot")
                        .HasColumnName("isBot");

                    b.Property<string>("LanguageCode")
                        .HasColumnName("languageCode");

                    b.Property<string>("LastName")
                        .HasColumnName("lastName");

                    b.Property<string>("Username")
                        .HasColumnName("userName");

                    b.HasKey("UserId");

                    b.HasIndex("UserId");

                    b.ToTable("user");
                });

            modelBuilder.Entity("TriggMine.ChatBot.Repository.Models.Message", b =>
                {
                    b.HasOne("TriggMine.ChatBot.Repository.Models.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
