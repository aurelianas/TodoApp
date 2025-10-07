using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoApp.PersistenceLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRefreshTokenfromUserCredentialtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "UserCredential");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpireDate",
                table: "UserCredential");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "UserCredential",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpireDate",
                table: "UserCredential",
                type: "datetime2",
                nullable: true);
        }
    }
}
