using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBuilderApp.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EvaluatedAt",
                table: "Resumes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Resumes",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvaluatedAt",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "Resumes");
        }
    }
}
