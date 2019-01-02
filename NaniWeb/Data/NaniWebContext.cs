using Microsoft.AspNetCore.Identity;
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
        public DbSet<MangadexSeries> MangadexSeries { get; set; }
        public DbSet<MangadexChapter> MangadexChapters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("NaniWebDb"));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ForNpgsqlUseIdentityColumns();
            builder.ForNpgsqlHasEnum<Series.SeriesType>(null, null, new NpgsqlNullNameTranslator());
            builder.ForNpgsqlHasEnum<Series.SeriesStatus>(null, null, new NpgsqlNullNameTranslator());
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
                entity.Property(series => series.IsVisible).IsRequired();
                entity.Property(series => series.Type).IsRequired();
                entity.Property(series => series.Status).IsRequired();
                entity.Property(series => series.UrlSlug).IsRequired();
                entity.HasIndex(series => series.UrlSlug).IsUnique();
                entity.HasMany(series => series.Chapters).WithOne(chapter => chapter.Series).HasForeignKey(chapter => chapter.SeriesId);
                entity.HasOne(series => series.MangadexInfo).WithOne(info => info.Series).HasForeignKey<MangadexSeries>(info => info.SeriesId);
            });

            builder.Entity<Chapter>(entity =>
            {
                entity.Property(chapter => chapter.ChapterNumber).IsRequired();
                entity.Property(chapter => chapter.IsVisible).IsRequired();
                entity.Property(chapter => chapter.SeriesId).IsRequired();
                entity.HasIndex(chapter => new {chapter.ChapterNumber, chapter.SeriesId}).IsUnique();
                entity.HasMany(chapter => chapter.Pages).WithOne(page => page.Chapter).HasForeignKey(page => page.ChapterId);
                entity.HasOne(chapter => chapter.MangadexInfo).WithOne(info => info.Chapter).HasForeignKey<MangadexChapter>(info => info.ChapterId);
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
                    Value = "localhost"
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
                    Id = 9,
                    Name = "EnableDiscordBot",
                    Value = false.ToString()
                }, new Setting
                {
                    Id = 10,
                    Name = "DiscordToken",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 11,
                    Name = "DiscordChannelId",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 12,
                    Name = "EnableMangadexAutoUpload",
                    Value = false.ToString()
                }, new Setting
                {
                    Id = 13,
                    Name = "MangadexUser",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 14,
                    Name = "MangadexPassword",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 15,
                    Name = "MangadexGroupId",
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
                    Id = 18,
                    Name = "EnableFcm",
                    Value = false.ToString()
                }, new Setting
                {
                    Id = 19,
                    Name = "FcmApiKey",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 20,
                    Name = "FcmProjectId",
                    Value = string.Empty
                }, new Setting
                {
                    Id = 21,
                    Name = "FcmSenderId",
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
                });
            });

            builder.Entity<MangadexSeries>(entity =>
            {
                entity.Property(info => info.SeriesId).IsRequired();
                entity.HasIndex(info => info.SeriesId).IsUnique();
                entity.Property(info => info.MangadexId).IsRequired();
                entity.HasIndex(info => info.MangadexId).IsUnique();
            });

            builder.Entity<MangadexChapter>(entity =>
            {
                entity.Property(info => info.ChapterId).IsRequired();
                entity.HasIndex(info => info.ChapterId).IsUnique();
                entity.Property(info => info.MangadexId).IsRequired();
                entity.HasIndex(info => info.MangadexId).IsUnique();
            });
        }
    }
}