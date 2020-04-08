using Microsoft.EntityFrameworkCore.Migrations;

namespace RentSpace.Migrations
{
    public partial class ColumnsaddedtoAppointmentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PropertyId",
                table: "Appointment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Appointment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PropertyId",
                table: "Appointment",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Property_PropertyId",
                table: "Appointment",
                column: "PropertyId",
                principalTable: "Property",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Property_PropertyId",
                table: "Appointment");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_PropertyId",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Appointment");
        }
    }
}
