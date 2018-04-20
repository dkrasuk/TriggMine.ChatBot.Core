using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TriggMine.ChatBot.Repository.Migrations
{
    public partial class addColumnTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "chatTitle",
                schema: "ChatBot",
                table: "message",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "chatTitle",
                schema: "ChatBot",
                table: "message");
        }
    }
}
