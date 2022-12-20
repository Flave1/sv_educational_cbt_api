using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBT.DAL.Migrations
{
    public partial class CandidateIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CandidateIds",
                table: "Examination",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandidateIds",
                table: "Examination");
        }
    }
}
