using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass()]
    public class ManutencaoServiceTests
    {
        private AluguelinkContext context = null!;
        private IManutencaoService manutencaoService = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var builder = new DbContextOptionsBuilder<AluguelinkContext>();
            builder.UseInMemoryDatabase("aluguelinkdb_manutencao");
            var options = builder.Options;

            context = new AluguelinkContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Setup básico para testes futuros
            manutencaoService = new Service.ManutencaoService(context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // TODO: Implementar teste de criação de manutenção
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void GetTest()
        {
            // TODO: Implementar teste de busca de manutenção
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // TODO: Implementar teste de listagem de manutenções
            Assert.IsTrue(true);
        }
    }
}