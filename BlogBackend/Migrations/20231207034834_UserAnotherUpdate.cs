using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogBackend.Migrations
{
    /// <inheritdoc />
    public partial class UserAnotherUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Communities",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Communities",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Communities_UserId",
                table: "Communities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Communities_Users_UserId",
                table: "Communities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Communities_Users_UserId",
                table: "Communities");

            migrationBuilder.DropIndex(
                name: "IX_Communities_UserId",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Communities");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Communities",
                table: "Users",
                type: "uuid[]",
                nullable: false);
        }
    }
}
