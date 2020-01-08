using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NaniWeb.Data;
using NaniWeb.Others;
using NaniWeb.Others.Services;
using Npgsql;

namespace NaniWeb
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql().AddDbContext<NaniWebContext>();

            services.AddIdentity<IdentityUser<int>, IdentityRole<int>>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(3);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<NaniWebContext>().AddDefaultTokenProviders();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllersWithViews(options => options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())))
                .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false).SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/admin/";
                options.LogoutPath = "/admin/signout";
                options.AccessDeniedPath = "/denied";
            });

            services.AddDataProtection().PersistKeysToFileSystem(Directory.CreateDirectory($"{Utils.CurrentDirectory.FullName}{Path.DirectorySeparatorChar}Keys"))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(30))
                .ProtectKeysWithCertificate(Utils.GetCertificate("keycert.pfx"));

            services.AddSingleton(typeof(SettingsKeeper));
            services.AddTransient(typeof(ReCaptcha));
            services.AddTransient<EmailSender>();
            services.AddTransient(typeof(DiscordBot));
            services.AddTransient(typeof(RedditPoster));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture);
                options.SupportedCultures = new List<CultureInfo>(new[] {CultureInfo.InvariantCulture});
                options.SupportedUICultures = new List<CultureInfo>(new[] {CultureInfo.InvariantCulture});
            });

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    "Others",
                    "{action}",
                    new {controller = "Others"});
                routes.MapControllerRoute(
                    "Home",
                    "{action}/{urlSlug?}/{chapterNumber:decimal?}",
                    new {controller = "Home", action = "Index"});
                routes.MapControllerRoute(
                    "AdminLogin",
                    "admin/{action}",
                    new {controller = "SignIn", action = "SignIn"});
                routes.MapControllerRoute(
                    "Profile",
                    "admin/profile/{action}",
                    new {controller = "Profile", action = "Index"});
                routes.MapControllerRoute(
                    "AnnouncementManager",
                    "/admin/manager/announcement/{action}/{id:int?}",
                    new {controller = "AnnouncementManager"});
                routes.MapControllerRoute(
                    "SeriesManager",
                    "/admin/manager/series/{action}/{id:int?}",
                    new {controller = "SeriesManager"});
                routes.MapControllerRoute(
                    "ChapterManager",
                    "/admin/manager/chapter/{action}/{id:int?}",
                    new {controller = "ChapterManager"});
                routes.MapControllerRoute(
                    "UserManager",
                    "/admin/manager/user/{action}/{id:int?}",
                    new {controller = "UserManager"});
                routes.MapControllerRoute(
                    "SettingsManager",
                    "/admin/manager/settings/{action}",
                    new {controller = "SettingsManager"});
            });

            if (Utils.IsInstalled())
                using (var scope = app.ApplicationServices.CreateScope())
                using (var context = scope.ServiceProvider.GetRequiredService<NaniWebContext>())
                {
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        context.Database.Migrate();
                        var npgsqlConnection = (NpgsqlConnection) context.Database.GetDbConnection();
                        npgsqlConnection.Open();
                        npgsqlConnection.ReloadTypes();
                    }
                }
        }
    }
}