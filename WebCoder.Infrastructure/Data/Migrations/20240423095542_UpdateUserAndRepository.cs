using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCoder.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAndRepository : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "ProjectRepositories",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "ProjectRepositories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "AspNetUsers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRepositories_OwnerId",
                table: "ProjectRepositories",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectRepositories_AspNetUsers_OwnerId",
                table: "ProjectRepositories",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectRepositories_AspNetUsers_OwnerId",
                table: "ProjectRepositories");

            migrationBuilder.DropIndex(
                name: "IX_ProjectRepositories_OwnerId",
                table: "ProjectRepositories");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "ProjectRepositories");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "ProjectRepositories");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "AspNetUsers");
        }
    }
}
