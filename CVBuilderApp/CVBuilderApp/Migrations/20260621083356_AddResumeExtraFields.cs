using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBuilderApp.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkedIn",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalSummary",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedIn",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "PersonalSummary",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "Resumes");
        }
    }
}
