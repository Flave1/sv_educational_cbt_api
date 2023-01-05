using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBT.DAL.Migrations
{
    public partial class IncludedExamAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowExamQuestionsAndAnswers",
                table: "Setting",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowExamQuestionsAndAnswers",
                table: "Setting");
        }
    }
}
