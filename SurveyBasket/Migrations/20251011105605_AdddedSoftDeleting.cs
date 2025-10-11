using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Migrations
{
    /// <inheritdoc />
    public partial class AdddedSoftDeleting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Polls",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Polls",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Polls",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Polls_DeletedById",
                table: "Polls",
                column: "DeletedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_AspNetUsers_DeletedById",
                table: "Polls",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Polls_AspNetUsers_DeletedById",
                table: "Polls");

            migrationBuilder.DropIndex(
                name: "IX_Polls_DeletedById",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Polls");
        }
    }
}
