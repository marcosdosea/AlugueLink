using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using AlugueLinkWEB.Areas.Identity.Data;

namespace AlugueLinkWEB.Helpers
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DataSeeder");
            try
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var userManager = sp.GetRequiredService<UserManager<UsuarioIdentity>>();
                var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

                // Garantir Role Administrator
                const string adminRole = "Administrator";
                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    await roleManager.CreateAsync(new IdentityRole(adminRole));
                    logger.LogInformation("Role {role} criada", adminRole);
                }

                var adminSection = config.GetSection("AdminUser");
                var adminEmail = adminSection["Email"] ?? "admin@aluguelink.com";
                var adminPassword = adminSection["Password"] ?? "Admin1234!";
                var adminNome = adminSection["NomeCompleto"] ?? "Administrador";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new UsuarioIdentity
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        NomeCompleto = adminNome,
                        Ativo = true,
                        DataCadastro = DateTime.UtcNow
                    };
                    var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                    if (!createResult.Succeeded)
                    {
                        logger.LogWarning("Falha ao criar usuário admin: {erros}", string.Join(",", createResult.Errors));
                    }
                    else
                    {
                        logger.LogInformation("Usuário admin criado: {email}", adminEmail);
                    }
                }

                // Garantir associação à role
                if (!await userManager.IsInRoleAsync(adminUser, adminRole))
                {
                    var addRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
                    if (!addRoleResult.Succeeded)
                    {
                        logger.LogWarning("Falha ao adicionar role admin: {erros}", string.Join(",", addRoleResult.Errors));
                    }
                }
            }
            catch (Exception ex)
            {
                // Apenas loga, não interrompe aplicação
                Console.WriteLine($"[DataSeeder] Erro: {ex.Message}");
            }
        }
    }
}
