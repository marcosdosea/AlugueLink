using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using AlugueLinkWEB.Areas.Identity.Data;
using Core.Service;
using Core;

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

                if (!await userManager.IsInRoleAsync(adminUser, adminRole))
                {
                    var addRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
                    if (!addRoleResult.Succeeded)
                    {
                        logger.LogWarning("Falha ao adicionar role admin: {erros}", string.Join(",", addRoleResult.Errors));
                    }
                }

                await SeedTestDataAsync(sp, logger);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DataSeeder] Erro: {ex.Message}");
            }
        }


        private static async Task SeedTestDataAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                var locadorService = serviceProvider.GetService<ILocadorService>();
                var imovelService = serviceProvider.GetService<IImovelService>();
                var locatarioService = serviceProvider.GetService<ILocatarioService>();

                if (locadorService?.GetAll(1, 1).Any() != true)
                {
                    var locadorTeste = new Locador
                    {
                        Nome = "João Silva",
                        Email = "joao@exemplo.com",
                        Telefone = "11987654321",
                        Cpf = "12345678901"
                    };

                    var locadorId = locadorService.Create(locadorTeste);
                    logger.LogInformation("Locador de teste criado: {nome}", locadorTeste.Nome);

                    if (imovelService != null)
                    {
                        var imovelTeste = new Imovel
                        {
                            IdLocador = locadorId,
                            Cep = "01234567",
                            Logradouro = "Rua das Flores",
                            Numero = "123",
                            Bairro = "Centro",
                            Cidade = "São Paulo",
                            Estado = "SP",
                            Tipo = "A", 
                            Quartos = 2,
                            Banheiros = 1,
                            Area = 60.00m,
                            VagasGaragem = 1,
                            Valor = 2500.00m,
                            Descricao = "Apartamento bem localizado no centro da cidade"
                        };

                        imovelService.Create(imovelTeste);
                        logger.LogInformation("Imóvel de teste criado");
                    }

                    if (locatarioService != null)
                    {
                        var locatarioTeste = new Locatario
                        {
                            Nome = "Maria Santos",
                            Email = "maria@exemplo.com",
                            Telefone1 = "11876543210",
                            Telefone2 = "11876543210",
                            Cpf = "98765432100",
                            Cep = "12345678",
                            Logradouro = "Rua das Acácias",
                            Numero = "456",
                            Bairro = "Jardim",
                            Cidade = "São Paulo",
                            Estado = "SP",
                            Profissao = "Professora",
                            Renda = 5000.00m
                        };

                        locatarioService.Create(locatarioTeste);
                        logger.LogInformation("Locatário de teste criado");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("Erro ao criar dados de teste: {erro}", ex.Message);
            }
        }
    }
}
