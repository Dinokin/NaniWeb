using Microsoft.EntityFrameworkCore.Migrations;

namespace NaniWeb.Data.Migrations
{
    public partial class RemoveFB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                "Settings",
                "Id",
                24,
                new[] {"Name", "Value"},
                new object[] {"RecaptchaSiteKey", ""});

            migrationBuilder.UpdateData(
                "Settings",
                "Id",
                25,
                "Name",
                "RecaptchaSecretKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                "Settings",
                "Id",
                24,
                new[] {"Name", "Value"},
                new object[] {"EnableFacebookPosting", "False"});

            migrationBuilder.UpdateData(
                "Settings",
                "Id",
                25,
                "Name",
                "FacebookApiKey");
        }
    }
}