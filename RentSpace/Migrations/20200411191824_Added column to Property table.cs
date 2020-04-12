using Microsoft.EntityFrameworkCore.Migrations;

namespace RentSpace.Migrations
{
    public partial class AddedcolumntoPropertytable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AppointmentRequested",
                table: "Property",
                nullable: true,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentRequested",
                table: "Property");
        }
    }
}
