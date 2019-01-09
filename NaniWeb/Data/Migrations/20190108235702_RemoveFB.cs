using Microsoft.EntityFrameworkCore.Migrations;

namespace NaniWeb.Data.Migrations
{
    public partial class RemoveFB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Name", "Value" },
                values: new object[] { "RecaptchaSiteKey", "" });

            migrationBuilder.UpdateData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "RecaptchaSecretKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Name", "Value" },
                values: new object[] { "EnableFacebookPosting", "False" });

            migrationBuilder.UpdateData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "FacebookApiKey");
        }
    }
}
