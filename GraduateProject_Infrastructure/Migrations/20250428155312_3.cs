using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduateProject_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComplianceLevel",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "FollowUpNeeded",
                table: "MedicalHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RecordedAt",
                table: "MedicalHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedRescheduleDate",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RescheduleReason",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RescheduleStatus",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WaitingMinutes",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientReports",
                columns: table => new
                {
                    PatientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientReports", x => x.PatientID);
                });

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    ProcedureID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorID = table.Column<int>(type: "int", nullable: false),
                    ProcedureType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    DatePerformed = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.ProcedureID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientReports");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropColumn(
                name: "ComplianceLevel",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "FollowUpNeeded",
                table: "MedicalHistories");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "MedicalHistories");

            migrationBuilder.DropColumn(
                name: "RecordedAt",
                table: "MedicalHistories");

            migrationBuilder.DropColumn(
                name: "RequestedRescheduleDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "RescheduleReason",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "RescheduleStatus",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "WaitingMinutes",
                table: "Appointments");
        }
    }
}
