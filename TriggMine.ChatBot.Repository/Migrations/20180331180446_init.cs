using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;
using System.Collections.Generic;

namespace TriggMine.ChatBot.Repository.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ChatBot");

            migrationBuilder.CreateTable(
                name: "user",
                schema: "ChatBot",
                columns: table => new
                {
                    userId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    dateFirstActivity = table.Column<DateTime>(nullable: false, defaultValueSql: "(now() at time zone 'utc')"),
                    firsName = table.Column<string>(nullable: true),
                    isBlocked = table.Column<bool>(nullable: false),
                    isBot = table.Column<bool>(nullable: false),
                    languageCode = table.Column<string>(nullable: true),
                    lastName = table.Column<string>(nullable: true),
                    userName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "message",
                schema: "ChatBot",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    chatId = table.Column<long>(nullable: false),
                    messageId = table.Column<int>(nullable: false),
                    MessageSendDate = table.Column<DateTime>(nullable: false),
                    sendMessageDate = table.Column<DateTime>(nullable: false, defaultValueSql: "(now() at time zone 'utc')"),
                    text = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message", x => x.id);
                    table.ForeignKey(
                        name: "FK_message_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "ChatBot",
                        principalTable: "user",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_message_chatId",
                schema: "ChatBot",
                table: "message",
                column: "chatId");

            migrationBuilder.CreateIndex(
                name: "IX_message_id",
                schema: "ChatBot",
                table: "message",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_message_messageId",
                schema: "ChatBot",
                table: "message",
                column: "messageId");

            migrationBuilder.CreateIndex(
                name: "IX_message_UserId",
                schema: "ChatBot",
                table: "message",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_userId",
                schema: "ChatBot",
                table: "user",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "message",
                schema: "ChatBot");

            migrationBuilder.DropTable(
                name: "user",
                schema: "ChatBot");
        }
    }
}
