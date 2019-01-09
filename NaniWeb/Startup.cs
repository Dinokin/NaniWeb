using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NaniWeb.Data;
using NaniWeb.Others;
using NaniWeb.Others.Services;
using Npgsql;

namespace NaniWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql().AddDbContext<NaniWebContext>().BuildServiceProvider();

            services.AddIdentity<IdentityUser<int>, IdentityRole<int>>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
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

            services.AddResponseCompression();
            services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
            services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });

            services.AddMvc(options => options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())))
                .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false).SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/admin/";
                options.LogoutPath = "/admin/signout";
                options.AccessDeniedPath = "/admin/denied";
            });

            services.AddHttpClient("MangadexClient", client =>
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("NaniWeb/1.0");
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                client.BaseAddress = new Uri(MangadexUploader.MangadexAddress);
                client.DefaultRequestHeaders.Host = "mangadex.org";
                client.DefaultRequestHeaders.Referrer = new Uri(MangadexUploader.MangadexAddress);
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            });

            services.AddSingleton(typeof(SettingsKeeper));
            services.AddTransient(typeof(ReCaptcha));
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient(typeof(DiscordBot));
            services.AddTransient(typeof(MangadexUploader));
            services.AddTransient(typeof(FirebaseCloudMessaging));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "Others",
                    "{action}",
                    new {controller = "Others"});
                routes.MapRoute(
                    "Home",
                    "{action}/{urlSlug?}/{chapterNumber:decimal?}",
                    new {controller = "Home", action = "Index"});
                routes.MapRoute(
                    "Subscription",
                    "subscription/{action}/{topic}/{token}",
                    new {controller = "Subscription"});
                routes.MapRoute(
                    "AdminLogin",
                    "admin/{action}",
                    new {controller = "SignIn", action = "SignIn"});
                routes.MapRoute(
                    "Profile",
                    "admin/profile/{action}",
                    new {controller = "Profile", action = "Index"});
                routes.MapRoute(
                    "AnnouncementManager",
                    "/admin/manager/announcement/{action}/{id:int?}",
                    new {controller = "AnnouncementManager"});
                routes.MapRoute(
                    "SeriesManager",
                    "/admin/manager/series/{action}/{id:int?}",
                    new {controller = "SeriesManager"});
                routes.MapRoute(
                    "ChapterManager",
                    "/admin/manager/chapter/{action}/{id:int?}",
                    new {controller = "ChapterManager"});
                routes.MapRoute(
                    "UserManager",
                    "/admin/manager/user/{action}/{id:int?}",
                    new {controller = "UserManager"});
                routes.MapRoute(
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