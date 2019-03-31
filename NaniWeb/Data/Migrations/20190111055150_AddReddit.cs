using Microsoft.EntityFrameworkCore.Migrations;

namespace NaniWeb.Data.Migrations
{
    public partial class AddReddit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "Settings",
                new[] {"Id", "Name", "Value"},
                new object[,]
                {
                    {30, "EnableReddit", "False"},
                    {31, "RedditUser", ""},
                    {32, "RedditPassword", ""},
                    {33, "RedditClientId", ""},
                    {34, "RedditClientSecret", ""}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                30);

            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                31);

            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                32);

            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                33);

            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                34);
        }
    }
}