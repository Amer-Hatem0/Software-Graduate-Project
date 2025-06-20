using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduateProject_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateadmin9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "OTPVerifications");

            migrationBuilder.CreateTable(
                name: "ChangePasswordModel",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OldPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangePasswordModel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ResetPasswordWithOTPModel",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OTP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetPasswordWithOTPModel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SendOTPModel",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendOTPModel", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangePasswordModel");

            migrationBuilder.DropTable(
                name: "ResetPasswordWithOTPModel");

            migrationBuilder.DropTable(
                name: "SendOTPModel");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "OTPVerifications",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
