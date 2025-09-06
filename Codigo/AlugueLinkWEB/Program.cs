using Core;
using Core.Service;
using AlugueLinkWEB.Filter;
using Microsoft.EntityFrameworkCore;
using Service;
using System.Globalization;

namespace AlugueLinkWEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuração de cultura para suportar vírgula como separador decimal
            var cultureInfo = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<CustomExceptionFilter>();
            });

            // Services - seguindo padrão do CuidaPet com Transient
            builder.Services.AddTransient<ILocatarioService, LocatarioService>();
            builder.Services.AddTransient<IImovelService, ImovelService>();
            builder.Services.AddTransient<ILocadorService, LocadorService>();
            builder.Services.AddTransient<IAluguelService, AluguelService>();
            builder.Services.AddTransient<IPagamentoService, PagamentoService>();
            builder.Services.AddTransient<IManutencaoService, ManutencaoService>();

            // AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Database
            builder.Services.AddDbContext<AluguelinkContext>(
                options => options.UseMySql(
                    builder.Configuration.GetConnectionString("AluguelinkDatabase"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("AluguelinkDatabase"))
                ));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

                app.UseHsts();
            }

            // Configurar localização
            var supportedCultures = new[] { "pt-BR" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}