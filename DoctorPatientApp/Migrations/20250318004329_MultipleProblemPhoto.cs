using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorPatientApp.Migrations
{
    /// <inheritdoc />
    public partial class MultipleProblemPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProblemPhotoPath2",
                table: "Examinations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProblemPhotoPath3",
                table: "Examinations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProblemPhotoPath4",
                table: "Examinations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProblemPhotoPath2",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "ProblemPhotoPath3",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "ProblemPhotoPath4",
                table: "Examinations");
        }
    }
}
