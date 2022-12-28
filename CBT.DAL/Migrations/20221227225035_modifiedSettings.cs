using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBT.DAL.Migrations
{
    public partial class modifiedSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadToSmpAsAssessment",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "UploadToSmpAsExam",
                table: "Setting");

            migrationBuilder.AddColumn<bool>(
                name: "UploadToSmpAsAssessment",
                table: "Examination",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UploadToSmpAsExam",
                table: "Examination",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadToSmpAsAssessment",
                table: "Examination");

            migrationBuilder.DropColumn(
                name: "UploadToSmpAsExam",
                table: "Examination");

            migrationBuilder.AddColumn<bool>(
                name: "UploadToSmpAsAssessment",
                table: "Setting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UploadToSmpAsExam",
                table: "Setting",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
