using Microsoft.EntityFrameworkCore.Migrations;

namespace RentSpace.Migrations
{
    public partial class Chnagestotable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Property");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Property",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Property");

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Property",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
