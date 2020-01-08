using Microsoft.EntityFrameworkCore.Migrations;

namespace NaniWeb.Data.Migrations
{
    public partial class RemoveFcmIntegration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 21);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[,]
                {
                    { 18, "EnableFcm", "False" },
                    { 19, "FcmApiKey", "" },
                    { 20, "FcmProjectId", "" },
                    { 21, "FcmSenderId", "0" }
                });
        }
    }
}
