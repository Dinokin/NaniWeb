using Microsoft.EntityFrameworkCore.Migrations;

namespace NaniWeb.Data.Migrations
{
    public partial class MoreSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "Settings",
                new[] {"Id", "Name", "Value"},
                new object[,]
                {
                    {28, "SiteSideBar", ""},
                    {29, "SiteAboutPage", ""}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                28);

            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                29);
        }
    }
}