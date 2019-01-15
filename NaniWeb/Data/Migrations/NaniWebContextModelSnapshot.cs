﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NaniWeb.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NaniWeb.Data.Migrations
{
    [DbContext(typeof(NaniWebContext))]
    partial class NaniWebContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:Enum:SeriesStatus", "Ongoing,Hiatus,Dropped,Completed")
                .HasAnnotation("Npgsql:Enum:SeriesType", "Manga,Webtoon")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConcurrencyStamp = "3cad4e5f-571d-423f-93ab-0b8937e43c73",
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR"
                        },
                        new
                        {
                            Id = 2,
                            ConcurrencyStamp = "bda6f3a2-e3e2-4862-95e0-9e5b78a37696",
                            Name = "Moderator",
                            NormalizedName = "MODERATOR"
                        },
                        new
                        {
                            Id = 3,
                            ConcurrencyStamp = "53ce78d8-56ea-4aaf-ab3b-c9d3c3bf6c54",
                            Name = "Uploader",
                            NormalizedName = "UPLOADER"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("NaniWeb.Data.Announcement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<DateTime>("PostDate");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<string>("UrlSlug")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.HasIndex("UrlSlug")
                        .IsUnique();

                    b.ToTable("Announcements");
                });

            modelBuilder.Entity("NaniWeb.Data.Chapter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("ChapterNumber");

                    b.Property<string>("Name");

                    b.Property<DateTime>("ReleaseDate");

                    b.Property<int>("SeriesId");

                    b.Property<int>("Volume");

                    b.HasKey("Id");

                    b.HasIndex("SeriesId");

                    b.HasIndex("ChapterNumber", "SeriesId")
                        .IsUnique();

                    b.ToTable("Chapters");
                });

            modelBuilder.Entity("NaniWeb.Data.MangadexChapter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChapterId");

                    b.Property<int>("MangadexId");

                    b.HasKey("Id");

                    b.HasIndex("ChapterId")
                        .IsUnique();

                    b.ToTable("MangadexChapters");
                });

            modelBuilder.Entity("NaniWeb.Data.MangadexSeries", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("MangadexId");

                    b.Property<int>("SeriesId");

                    b.HasKey("Id");

                    b.HasIndex("SeriesId")
                        .IsUnique();

                    b.ToTable("MangadexSeries");
                });

            modelBuilder.Entity("NaniWeb.Data.Page", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChapterId");

                    b.Property<int>("PageNumber");

                    b.HasKey("Id");

                    b.HasIndex("ChapterId");

                    b.HasIndex("PageNumber", "ChapterId")
                        .IsUnique();

                    b.ToTable("Pages");
                });

            modelBuilder.Entity("NaniWeb.Data.Series", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Artist")
                        .IsRequired();

                    b.Property<string>("Author")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Series.SeriesStatus>("Status");

                    b.Property<string>("Synopsis")
                        .IsRequired();

                    b.Property<Series.SeriesType>("Type");

                    b.Property<string>("UrlSlug")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UrlSlug")
                        .IsUnique();

                    b.ToTable("Series");
                });

            modelBuilder.Entity("NaniWeb.Data.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Settings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "SiteName",
                            Value = "NaniWeb"
                        },
                        new
                        {
                            Id = 2,
                            Name = "SiteDescription",
                            Value = "NaniWeb for scanlation groups."
                        },
                        new
                        {
                            Id = 3,
                            Name = "SiteUrl",
                            Value = "http://localhost"
                        },
                        new
                        {
                            Id = 4,
                            Name = "EnableRegistration",
                            Value = "False"
                        },
                        new
                        {
                            Id = 5,
                            Name = "EnableEmailRecovery",
                            Value = "False"
                        },
                        new
                        {
                            Id = 6,
                            Name = "SmtpServer",
                            Value = ""
                        },
                        new
                        {
                            Id = 7,
                            Name = "SmtpUser",
                            Value = ""
                        },
                        new
                        {
                            Id = 8,
                            Name = "SmtpPassword",
                            Value = ""
                        },
                        new
                        {
                            Id = 9,
                            Name = "EnableDiscordBot",
                            Value = "False"
                        },
                        new
                        {
                            Id = 10,
                            Name = "DiscordToken",
                            Value = ""
                        },
                        new
                        {
                            Id = 11,
                            Name = "DiscordChannelId",
                            Value = "0"
                        },
                        new
                        {
                            Id = 12,
                            Name = "EnableMangadexAutoUpload",
                            Value = "False"
                        },
                        new
                        {
                            Id = 13,
                            Name = "MangadexUser",
                            Value = ""
                        },
                        new
                        {
                            Id = 14,
                            Name = "MangadexPassword",
                            Value = ""
                        },
                        new
                        {
                            Id = 15,
                            Name = "MangadexGroupId",
                            Value = "0"
                        },
                        new
                        {
                            Id = 16,
                            Name = "EnableGoogleAnalytics",
                            Value = "False"
                        },
                        new
                        {
                            Id = 17,
                            Name = "GoogleAnalyticsTrackingCode",
                            Value = ""
                        },
                        new
                        {
                            Id = 18,
                            Name = "EnableFcm",
                            Value = "False"
                        },
                        new
                        {
                            Id = 19,
                            Name = "FcmApiKey",
                            Value = ""
                        },
                        new
                        {
                            Id = 20,
                            Name = "FcmProjectId",
                            Value = ""
                        },
                        new
                        {
                            Id = 21,
                            Name = "FcmSenderId",
                            Value = "0"
                        },
                        new
                        {
                            Id = 22,
                            Name = "EnableDisqus",
                            Value = "False"
                        },
                        new
                        {
                            Id = 23,
                            Name = "DisqusShortname",
                            Value = ""
                        },
                        new
                        {
                            Id = 24,
                            Name = "RecaptchaSiteKey",
                            Value = ""
                        },
                        new
                        {
                            Id = 25,
                            Name = "RecaptchaSecretKey",
                            Value = ""
                        },
                        new
                        {
                            Id = 26,
                            Name = "GroupsEmailAddress",
                            Value = ""
                        },
                        new
                        {
                            Id = 27,
                            Name = "SiteFooterCode",
                            Value = ""
                        },
                        new
                        {
                            Id = 28,
                            Name = "SiteSideBar",
                            Value = ""
                        },
                        new
                        {
                            Id = 29,
                            Name = "SiteAboutPage",
                            Value = ""
                        },
                        new
                        {
                            Id = 30,
                            Name = "EnableReddit",
                            Value = "False"
                        },
                        new
                        {
                            Id = 31,
                            Name = "RedditUser",
                            Value = ""
                        },
                        new
                        {
                            Id = 32,
                            Name = "RedditPassword",
                            Value = ""
                        },
                        new
                        {
                            Id = 33,
                            Name = "RedditClientId",
                            Value = ""
                        },
                        new
                        {
                            Id = 34,
                            Name = "RedditClientSecret",
                            Value = ""
                        },
                        new
                        {
                            Id = 35,
                            Name = "EnableAds",
                            Value = "False"
                        },
                        new
                        {
                            Id = 36,
                            Name = "AdsHeadScripts",
                            Value = ""
                        },
                        new
                        {
                            Id = 37,
                            Name = "AdsBodyScripts",
                            Value = ""
                        },
                        new
                        {
                            Id = 38,
                            Name = "AdsContainerCode",
                            Value = ""
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<int>")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<int>")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<int>")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser<int>")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NaniWeb.Data.Chapter", b =>
                {
                    b.HasOne("NaniWeb.Data.Series", "Series")
                        .WithMany("Chapters")
                        .HasForeignKey("SeriesId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NaniWeb.Data.MangadexChapter", b =>
                {
                    b.HasOne("NaniWeb.Data.Chapter", "Chapter")
                        .WithOne("MangadexInfo")
                        .HasForeignKey("NaniWeb.Data.MangadexChapter", "ChapterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NaniWeb.Data.MangadexSeries", b =>
                {
                    b.HasOne("NaniWeb.Data.Series", "Series")
                        .WithOne("MangadexInfo")
                        .HasForeignKey("NaniWeb.Data.MangadexSeries", "SeriesId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NaniWeb.Data.Page", b =>
                {
                    b.HasOne("NaniWeb.Data.Chapter", "Chapter")
                        .WithMany("Pages")
                        .HasForeignKey("ChapterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
