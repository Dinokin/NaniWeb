using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NaniWeb.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:SeriesStatus", "Ongoing,Hiatus,Dropped,Completed")
                .Annotation("Npgsql:Enum:SeriesType", "Manga,Webtoon");

            migrationBuilder.CreateTable(
                "Announcements",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    UrlSlug = table.Column<string>(nullable: false),
                    PostDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Announcements", x => x.Id); });

            migrationBuilder.CreateTable(
                "Roles",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Roles", x => x.Id); });

            migrationBuilder.CreateTable(
                "Series",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false),
                    Author = table.Column<string>(nullable: false),
                    Artist = table.Column<string>(nullable: false),
                    Synopsis = table.Column<string>(nullable: false),
                    IsVisible = table.Column<bool>(nullable: false),
                    Type = table.Column<Series.SeriesType>(nullable: false),
                    Status = table.Column<Series.SeriesStatus>(nullable: false),
                    UrlSlug = table.Column<string>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Series", x => x.Id); });

            migrationBuilder.CreateTable(
                "Settings",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Settings", x => x.Id); });

            migrationBuilder.CreateTable(
                "Users",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Users", x => x.Id); });

            migrationBuilder.CreateTable(
                "RoleClaims",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_RoleClaims_Roles_RoleId",
                        x => x.RoleId,
                        "Roles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Chapters",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Volume = table.Column<int>(nullable: false),
                    ChapterNumber = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsVisible = table.Column<bool>(nullable: false),
                    SeriesId = table.Column<int>(nullable: false),
                    ReleaseDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                    table.ForeignKey(
                        "FK_Chapters_Series_SeriesId",
                        x => x.SeriesId,
                        "Series",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "MangadexSeries",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeriesId = table.Column<int>(nullable: false),
                    MangadexId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangadexSeries", x => x.Id);
                    table.ForeignKey(
                        "FK_MangadexSeries_Series_SeriesId",
                        x => x.SeriesId,
                        "Series",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "UserClaims",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_UserClaims_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "UserLogins",
                table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new {x.LoginProvider, x.ProviderKey});
                    table.ForeignKey(
                        "FK_UserLogins_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "UserRoles",
                table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new {x.UserId, x.RoleId});
                    table.ForeignKey(
                        "FK_UserRoles_Roles_RoleId",
                        x => x.RoleId,
                        "Roles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_UserRoles_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "UserTokens",
                table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new {x.UserId, x.LoginProvider, x.Name});
                    table.ForeignKey(
                        "FK_UserTokens_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "MangadexChapters",
                table => new
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
                        "FK_MangadexChapters_Chapters_ChapterId",
                        x => x.ChapterId,
                        "Chapters",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Pages",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PageNumber = table.Column<int>(nullable: false),
                    ChapterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                    table.ForeignKey(
                        "FK_Pages_Chapters_ChapterId",
                        x => x.ChapterId,
                        "Chapters",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                "Roles",
                new[] {"Id", "ConcurrencyStamp", "Name", "NormalizedName"},
                new object[,]
                {
                    {1, "3cad4e5f-571d-423f-93ab-0b8937e43c73", "Administrator", "ADMINISTRATOR"},
                    {2, "bda6f3a2-e3e2-4862-95e0-9e5b78a37696", "Moderator", "MODERATOR"},
                    {3, "53ce78d8-56ea-4aaf-ab3b-c9d3c3bf6c54", "Uploader", "UPLOADER"}
                });

            migrationBuilder.InsertData(
                "Settings",
                new[] {"Id", "Name", "Value"},
                new object[,]
                {
                    {23, "DisqusShortname", ""},
                    {22, "EnableDisqus", "False"},
                    {21, "FcmSenderId", "0"},
                    {20, "FcmProjectId", ""},
                    {19, "FcmApiKey", ""},
                    {18, "EnableFcm", "False"},
                    {17, "GoogleAnalyticsTrackingCode", ""},
                    {16, "EnableGoogleAnalytics", "False"},
                    {15, "MangadexGroupId", "0"},
                    {14, "MangadexPassword", ""},
                    {13, "MangadexUser", ""},
                    {11, "DiscordChannelId", "0"},
                    {24, "EnableFacebookPosting", "False"},
                    {10, "DiscordToken", ""},
                    {9, "EnableDiscordBot", "False"},
                    {8, "SmtpPassword", ""},
                    {7, "SmtpUser", ""},
                    {6, "SmtpServer", ""},
                    {5, "EnableEmailRecovery", "False"},
                    {4, "EnableRegistration", "False"},
                    {3, "SiteUrl", "http://localhost"},
                    {2, "SiteDescription", "NaniWeb for scanlation groups."},
                    {1, "SiteName", "NaniWeb"},
                    {12, "EnableMangadexAutoUpload", "False"},
                    {25, "FacebookApiKey", ""}
                });

            migrationBuilder.CreateIndex(
                "IX_Announcements_Title",
                "Announcements",
                "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Announcements_UrlSlug",
                "Announcements",
                "UrlSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Chapters_SeriesId",
                "Chapters",
                "SeriesId");

            migrationBuilder.CreateIndex(
                "IX_Chapters_ChapterNumber_SeriesId",
                "Chapters",
                new[] {"ChapterNumber", "SeriesId"},
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_MangadexChapters_ChapterId",
                "MangadexChapters",
                "ChapterId",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_MangadexChapters_MangadexId",
                "MangadexChapters",
                "MangadexId",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_MangadexSeries_MangadexId",
                "MangadexSeries",
                "MangadexId",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_MangadexSeries_SeriesId",
                "MangadexSeries",
                "SeriesId",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Pages_ChapterId",
                "Pages",
                "ChapterId");

            migrationBuilder.CreateIndex(
                "IX_Pages_PageNumber_ChapterId",
                "Pages",
                new[] {"PageNumber", "ChapterId"},
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_RoleClaims_RoleId",
                "RoleClaims",
                "RoleId");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "Roles",
                "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Series_Name",
                "Series",
                "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Series_UrlSlug",
                "Series",
                "UrlSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Settings_Name",
                "Settings",
                "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_UserClaims_UserId",
                "UserClaims",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_UserLogins_UserId",
                "UserLogins",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_UserRoles_RoleId",
                "UserRoles",
                "RoleId");

            migrationBuilder.CreateIndex(
                "EmailIndex",
                "Users",
                "NormalizedEmail");

            migrationBuilder.CreateIndex(
                "UserNameIndex",
                "Users",
                "NormalizedUserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Announcements");

            migrationBuilder.DropTable(
                "MangadexChapters");

            migrationBuilder.DropTable(
                "MangadexSeries");

            migrationBuilder.DropTable(
                "Pages");

            migrationBuilder.DropTable(
                "RoleClaims");

            migrationBuilder.DropTable(
                "Settings");

            migrationBuilder.DropTable(
                "UserClaims");

            migrationBuilder.DropTable(
                "UserLogins");

            migrationBuilder.DropTable(
                "UserRoles");

            migrationBuilder.DropTable(
                "UserTokens");

            migrationBuilder.DropTable(
                "Chapters");

            migrationBuilder.DropTable(
                "Roles");

            migrationBuilder.DropTable(
                "Users");

            migrationBuilder.DropTable(
                "Series");
        }
    }
}