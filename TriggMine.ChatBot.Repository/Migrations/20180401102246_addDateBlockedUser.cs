using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TriggMine.ChatBot.Repository.Migrations
{
    public partial class addDateBlockedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "isBot",
                schema: "ChatBot",
                table: "user",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "isBlocked",
                schema: "ChatBot",
                table: "user",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<DateTime>(
                name: "dateBlockedUser",
                schema: "ChatBot",
                table: "user",
                nullable: true,
                defaultValueSql: "(now() at time zone 'utc')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dateBlockedUser",
                schema: "ChatBot",
                table: "user");

            migrationBuilder.AlterColumn<bool>(
                name: "isBot",
                schema: "ChatBot",
                table: "user",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "isBlocked",
                schema: "ChatBot",
                table: "user",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
