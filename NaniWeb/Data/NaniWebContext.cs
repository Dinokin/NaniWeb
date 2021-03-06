﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Npgsql.NameTranslation;

namespace NaniWeb.Data
{
    public class NaniWebContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        private readonly IConfiguration _configuration;

        static NaniWebContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Series.SeriesType>(null, new NpgsqlNullNameTranslator());
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Series.SeriesStatus>(null, new NpgsqlNullNameTranslator());
        }

        public NaniWebContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("NaniWebDb"));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasPostgresEnum<Series.SeriesType>(null, null, new NpgsqlNullNameTranslator());
            builder.HasPostgresEnum<Series.SeriesStatus>(null, null, new NpgsqlNullNameTranslator());
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser<int>>().ToTable("Users");
            builder.Entity<IdentityRole<int>>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasData(new IdentityRole<int>
                {
                    Id = 1,
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    ConcurrencyStamp = "3cad4e5f-571d-423f-93ab-0b8937e43c73"
                }, new IdentityRole<int>
                {
                    Id = 2,
                    Name = "Moderator",
                    NormalizedName = "MODERATOR",
                    ConcurrencyStamp = "bda6f3a2-e3e2-4862-95e0-9e5b78a37696"
                }, new IdentityRole<int>
                {
                    Id = 3,
                    Name = "Uploader",
                    NormalizedName = "UPLOADER",
                    ConcurrencyStamp = "53ce78d8-56ea-4aaf-ab3b-c9d3c3bf6c54"
                });
            });
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            builder.Entity<Announcement>(entity =>
            {
                entity.Property(announcement => announcement.Title).IsRequired();
                entity.HasIndex(announcement => announcement.Title).IsUnique();
                entity.Property(announcement => announcement.Content).IsRequired();
                entity.Property(announcement => announcement.UrlSlug).IsRequired();
                entity.HasIndex(announcement => announcement.UrlSlug).IsUnique();
                entity.Property(announcement => announcement.PostDate).IsRequired();
            });

            builder.Entity<Series>(entity =>
            {
                entity.Property(series => series.Name).IsRequired();
                entity.HasIndex(series => series.Name).IsUnique();
                entity.Property(series => series.Author).IsRequired();
                entity.Property(series => series.Artist).IsRequired();
                entity.Property(series => series.Synopsis).IsRequired();
                entity.Property(series => series.Type).IsRequired();
                entity.Property(series => series.Status).IsRequired();
                entity.Property(series => series.UrlSlug).IsRequired();
                entity.HasIndex(series => series.UrlSlug).IsUnique();
                entity.HasMany(series => series.Chapters).WithOne(chapter => chapter.Series).HasForeignKey(chapter => chapter.SeriesId);
            });

            builder.Entity<Chapter>(entity =>
            {
                entity.Property(chapter => chapter.ChapterNumber).IsRequired();
                entity.Property(chapter => chapter.SeriesId).IsRequired();
                entity.HasIndex(chapter => new {chapter.ChapterNumber, chapter.SeriesId}).IsUnique();
                entity.HasMany(chapter => chapter.Pages).WithOne(page => page.Chapter).HasForeignKey(page => page.ChapterId);
            });

            builder.Entity<Page>(entity =>
            {
                entity.Property(page => page.PageNumber).IsRequired();
                entity.Property(page => page.ChapterId).IsRequired();
                entity.HasIndex(page => new {page.PageNumber, page.ChapterId}).IsUnique();
            });

            builder.Entity<Setting>(entity =>
            {
                entity.Property(settings => settings.Name).IsRequired();
                entity.HasIndex(settings => settings.Name).IsUnique();
                entity.Property(settings => settings.Value).IsRequired();
                entity.HasData(new Setting
                {
                    Id = 1,
                    Name = "SiteName",
                    Value = "NaniWeb"
                }, new Setting
                {
                    Id = 2,
                    Name = "SiteDescription",
                    Value = "NaniWeb for scanlation groups."
                }, new Setting
                {
                    Id = 3,
                    Name = "SiteUrl",
                    Value = "http://localhost"
                }, new Setting
                {
                    Id = 4,
                    Name = "EnableRegistration",
                    Value = false.ToString()
                }, new Setting
                {
                    Id = 5,
                    Name = "EnableEmailRecovery",
                    Value = false.ToString()
                }, new Setting
                {
                    Id = 6,
                    Name = "SmtpServer",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 7,
                    Name = "SmtpUser",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 8,
                    Name = "SmtpPassword",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 16,
                    Name = "EnableGoogleAnalytics",
                    Value = false.ToString()
                }, new Setting
                {
                    Id = 17,
                    Name = "GoogleAnalyticsTrackingCode",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 22,
                    Name = "EnableDisqus",
                    Value = false.ToString()
                }, new Setting
                {
                    Id = 23,
                    Name = "DisqusShortname",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 24,
                    Name = "RecaptchaSiteKey",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 25,
                    Name = "RecaptchaSecretKey",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 26,
                    Name = "GroupsEmailAddress",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 27,
                    Name = "SiteFooterCode",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 28,
                    Name = "SiteSideBar",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 29,
                    Name = "SiteAboutPage",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 35,
                    Name = "NumberOfUpdatesToShow",
                    Value = 10.ToString()
                }, new Setting
                {
                    Id = 36,
                    Name = "EnableAds",
                    Value = false.ToString()
                }, new Setting
                {
                    Id = 37,
                    Name = "AdsHeaderCode",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 38,
                    Name = "AdsLocationTop",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 39,
                    Name = "AdsLocationMiddle",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 40,
                    Name = "AdsLocationBottom",
                    Value = string.Empty
                });
            });
        }
    }
}