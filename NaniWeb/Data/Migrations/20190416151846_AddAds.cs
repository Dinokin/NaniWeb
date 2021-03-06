﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace NaniWeb.Data.Migrations
{
    public partial class AddAds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "Settings",
                new[] {"Id", "Name", "Value"},
                new object[,]
                {
                    {36, "EnableAds", "False"},
                    {37, "AdsHeaderCode", ""},
                    {38, "AdsLocationTop", ""},
                    {39, "AdsLocationMiddle", ""},
                    {40, "AdsLocationBottom", ""}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                36);

            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                37);

            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                38);

            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                39);

            migrationBuilder.DeleteData(
                "Settings",
                "Id",
                40);
        }
    }
}