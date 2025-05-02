using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduateProject_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AppointmentStatuses_StatusID",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_StatusID",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AppointmentStatusStatusID",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_StatusID",
                table: "Patients",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentStatusStatusID",
                table: "Appointments",
                column: "AppointmentStatusStatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AppointmentStatuses_AppointmentStatusStatusID",
                table: "Appointments",
                column: "AppointmentStatusStatusID",
                principalTable: "AppointmentStatuses",
                principalColumn: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AppointmentStatuses_StatusID",
                table: "Patients",
                column: "StatusID",
                principalTable: "AppointmentStatuses",
                principalColumn: "StatusID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AppointmentStatuses_AppointmentStatusStatusID",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AppointmentStatuses_StatusID",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_StatusID",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentStatusStatusID",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "AppointmentStatusStatusID",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_StatusID",
                table: "Appointments",
                column: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AppointmentStatuses_StatusID",
                table: "Appointments",
                column: "StatusID",
                principalTable: "AppointmentStatuses",
                principalColumn: "StatusID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
