using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBT.DAL.Migrations
{
    public partial class UpdatedSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Calculator",
                table: "Setting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GeoLocation",
                table: "Setting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ImageCasting",
                table: "Setting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ScreenRecording",
                table: "Setting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendToEmail",
                table: "Setting",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddColumn<bool>(
                name: "VideoRecording",
                table: "Setting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ViewCategory",
                table: "Setting",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calculator",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "GeoLocation",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "ImageCasting",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "ScreenRecording",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "SendToEmail",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "UploadToSmpAsAssessment",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "UploadToSmpAsExam",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "VideoRecording",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "ViewCategory",
                table: "Setting");
        }
    }
}
