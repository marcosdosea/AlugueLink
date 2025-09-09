using Core;
using Core.Service;
using AlugueLinkWEB.Filter;
using Microsoft.EntityFrameworkCore;
using Service;
using System.Globalization;
using AlugueLinkWEB.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AlugueLinkWEB.Helpers;

namespace AlugueLinkWEB
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var cultureInfo = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<CustomExceptionFilter>();
                options.Filters.Add<CustomAuthorizationFilter>();
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            builder.Services.AddRazorPages();

            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
                options.AddPolicy("OwnerOnly", policy => policy.RequireClaim("UserType", "Owner"));
            });

            builder.Services.AddTransient<ILocatarioService, LocatarioService>();
            builder.Services.AddTransient<IImovelService, ImovelService>();
            builder.Services.AddTransient<ILocadorService, LocadorService>();
            builder.Services.AddTransient<IAluguelService, AluguelService>();
            builder.Services.AddTransient<IPagamentoService, PagamentoService>();
            builder.Services.AddTransient<IManutencaoService, ManutencaoService>();

            builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();
            builder.Services.AddTransient<Core.Service.IEmailSender>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                return new EmailService(
                    configuration["Smtp:Host"]!,
                    int.Parse(configuration["Smtp:Port"]!),
                    configuration["Smtp:Username"]!,
                    configuration["Smtp:Password"]!,
                    configuration["Smtp:From"]!
                );
            });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddDbContext<AluguelinkContext>(
                options => options.UseMySQL(builder.Configuration.GetConnectionString("AluguelinkDatabase")!));

            builder.Services.AddDbContext<IdentityContext>(
                options => options.UseMySQL(builder.Configuration.GetConnectionString("IdentityDatabase")!));

            builder.Services.AddIdentity<UsuarioIdentity, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<IdentityContext>()
              .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.Cookie.Name = "AlugueLinkAuthCookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.Name = "__RequestVerificationToken";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            var supportedCultures = new[] { "pt-BR" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Executa seeding (roles + admin)
            await DataSeeder.SeedAsync(app.Services);

            await app.RunAsync();
        }
    }
}