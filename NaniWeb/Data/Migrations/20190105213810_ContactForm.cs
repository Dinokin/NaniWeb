using Microsoft.EntityFrameworkCore.Migrations;

namespace NaniWeb.Data.Migrations
{
    public partial class ContactForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 26, "GroupsEmailAddress", "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 26);
        }
    }
}
