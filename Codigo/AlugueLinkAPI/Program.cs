using AlugueLinkAPI.Filter;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;
using AlugueLinkAPI.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace AlugueLinkAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container with Exception handling.
            builder.Services.AddControllers(options => options.Filters.Add(new HttpResponseExceptionFilter()))
                .AddJsonOptions(options =>
                {
                    // Configurar para tratar referências circulares
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            builder.Services.AddTransient<ILocadorService, LocadorService>();
            builder.Services.AddTransient<ILocatarioService, LocatarioService>();
            builder.Services.AddTransient<IImovelService, ImovelService>();
            builder.Services.AddTransient<IAluguelService, AluguelService>();
            builder.Services.AddTransient<IPagamentoService, PagamentoService>();
            
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddDbContext<AluguelinkContext>(
                options => options.UseMySQL(builder.Configuration.GetConnectionString("AluguelinkDatabase")));

            builder.Services.AddDbContext<IdentityContext>(
                options => options.UseMySQL(builder.Configuration.GetConnectionString("IdentityDatabase")));

            builder.Services.AddIdentityApiEndpoints<UsuarioIdentity>(
                options =>
                {
                    // SignIn settings - Simplificando para teste
                    options.SignIn.RequireConfirmedAccount = false;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;

                    // Password settings - Mais flexível para teste
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 3;

                    // Default User settings.
                    options.User.AllowedUserNameCharacters =
                            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = false;

                    // Default Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                }).AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>();

            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapIdentityApi<UsuarioIdentity>();

            app.MapControllers();

            app.Run();
        }
    }
}
