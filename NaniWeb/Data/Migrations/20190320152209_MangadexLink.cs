using Microsoft.EntityFrameworkCore.Migrations;

namespace NaniWeb.Data.Migrations
{
    public partial class MangadexLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DisplayLink",
                table: "MangadexSeries",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayLink",
                table: "MangadexSeries");
        }
    }
}
