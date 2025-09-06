using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass()]
    public class LocadorServiceTests
    {
        private AluguelinkContext context = null!;
        private ILocadorService locadorService = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var builder = new DbContextOptionsBuilder<AluguelinkContext>();
            builder.UseInMemoryDatabase("aluguelinkdb_locador");
            var options = builder.Options;

            context = new AluguelinkContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var locadores = new List<Locador>
            {
                new() {
                    Id = 1,
                    Nome = "João Proprietário",
                    Email = "joao@gmail.com",
                    Cpf = "12345678901",
                    Telefone = "11999999999"
                },
                new() {
                    Id = 2,
                    Nome = "Maria Proprietária",
                    Email = "maria@gmail.com",
                    Cpf = "98765432100",
                    Telefone = "21987654321"
                }
            };

            context.AddRange(locadores);
            context.SaveChanges();

            locadorService = new Service.LocadorService(context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // TODO: Implementar teste de criação de locador
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void GetTest()
        {
            // TODO: Implementar teste de busca de locador
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // TODO: Implementar teste de listagem de locadores
            Assert.IsTrue(true);
        }
    }
}