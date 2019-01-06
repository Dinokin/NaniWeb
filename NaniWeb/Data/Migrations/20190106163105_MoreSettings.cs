using Microsoft.EntityFrameworkCore.Migrations;

namespace NaniWeb.Data.Migrations
{
    public partial class MoreSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[,]
                {
                    { 28, "SiteSideBar", "" },
                    { 29, "SiteAboutPage", "" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 29);
        }
    }
}
