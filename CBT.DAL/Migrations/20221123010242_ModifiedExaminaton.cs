using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBT.DAL.Migrations
{
    public partial class ModifiedExaminaton : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CandidateCategory_Class",
                table: "Examination",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ExamScore",
                table: "Examination",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandidateCategory_Class",
                table: "Examination");

            migrationBuilder.DropColumn(
                name: "ExamScore",
                table: "Examination");
        }
    }
}
