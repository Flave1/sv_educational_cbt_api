using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBT.DAL.Migrations
{
    public partial class SmsClientId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SmsClientId",
                table: "Setting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmsClientId",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmsClientId",
                table: "Examination",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmsClientId",
                table: "CandidateCategory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmsClientId",
                table: "CandidateAnswer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmsClientId",
                table: "Candidate",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SmsClientId",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "SmsClientId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "SmsClientId",
                table: "Examination");

            migrationBuilder.DropColumn(
                name: "SmsClientId",
                table: "CandidateCategory");

            migrationBuilder.DropColumn(
                name: "SmsClientId",
                table: "CandidateAnswer");

            migrationBuilder.DropColumn(
                name: "SmsClientId",
                table: "Candidate");
        }
    }
}
