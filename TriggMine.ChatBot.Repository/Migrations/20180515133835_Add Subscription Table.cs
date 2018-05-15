using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TriggMine.ChatBot.Repository.Migrations
{
    public partial class AddSubscriptionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "subscriptions",
                schema: "ChatBot",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    chatId = table.Column<long>(nullable: false),
                    dateSubscription = table.Column<DateTime>(nullable: false, defaultValueSql: "(now() at time zone 'utc')"),
                    subscriptionId = table.Column<int>(nullable: false),
                    subscriptionName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_chatId",
                schema: "ChatBot",
                table: "subscriptions",
                column: "chatId");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_subscriptionId",
                schema: "ChatBot",
                table: "subscriptions",
                column: "subscriptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscriptions",
                schema: "ChatBot");
        }
    }
}
