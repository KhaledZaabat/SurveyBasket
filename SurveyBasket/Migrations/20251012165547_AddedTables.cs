using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Migrations
{
    /// <inheritdoc />
    public partial class AddedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurveyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubmittedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSubmissions_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSubmissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoteId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    AnswerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionDetails_SurveyOptions_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "SurveyOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubmissionDetails_SurveyQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "SurveyQuestions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserSubmissions_SubmissionDetails_VoteId",
                        column: x => x.VoteId,
                        principalTable: "UserSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDetails_AnswerId",
                table: "SubmissionDetails",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDetails_QuestionId",
                table: "SubmissionDetails",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDetails_VoteId",
                table: "SubmissionDetails",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubmissions_SurveyId",
                table: "UserSubmissions",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubmissions_UserId",
                table: "UserSubmissions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionDetails");

            migrationBuilder.DropTable(
                name: "UserSubmissions");
        }
    }
}
