using Microsoft.EntityFrameworkCore.Migrations;

namespace NaniWeb.Data.Migrations
{
    public partial class UpdateCountSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "Settings",
                new[] {"Id", "Name", "Value"},
                new object[] {35, "NumberOfUpdatesToShow", "10"});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                35);
        }
    }
}