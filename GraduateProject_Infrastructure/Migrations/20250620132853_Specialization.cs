using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduateProject_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Specialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "ReportFiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "ReportFiles");
        }
    }
}
