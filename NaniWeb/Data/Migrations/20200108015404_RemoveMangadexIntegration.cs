using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NaniWeb.Data.Migrations
{
    public partial class RemoveMangadexIntegration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MangadexChapters");

            migrationBuilder.DropTable(
                name: "MangadexSeries");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 15);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MangadexChapters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChapterId = table.Column<int>(nullable: false),
                    MangadexId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangadexChapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MangadexChapters_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MangadexSeries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayLink = table.Column<bool>(nullable: false, defaultValue: false),
                    MangadexId = table.Column<int>(nullable: false),
                    SeriesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangadexSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MangadexSeries_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[,]
                {
                    { 12, "EnableMangadexAutoUpload", "False" },
                    { 13, "MangadexUser", "" },
                    { 14, "MangadexPassword", "" },
                    { 15, "MangadexGroupId", "0" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MangadexChapters_ChapterId",
                table: "MangadexChapters",
                column: "ChapterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MangadexSeries_SeriesId",
                table: "MangadexSeries",
                column: "SeriesId",
                unique: true);
        }
    }
}
