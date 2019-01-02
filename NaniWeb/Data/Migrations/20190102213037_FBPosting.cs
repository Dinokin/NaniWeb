using Microsoft.EntityFrameworkCore.Migrations;

namespace NaniWeb.Data.Migrations
{
    public partial class FBPosting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "Settings",
                new[] {"Id", "Name", "Value"},
                new object[,]
                {
                    {24, "EnableFacebookPosting", "False"},
                    {25, "FacebookApiKey", ""}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                24);

            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                25);
        }
    }
}