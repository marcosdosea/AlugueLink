using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;

namespace Service.Tests
{
    [TestClass()]
    public class AluguelServiceTests
    {
        private AluguelinkContext context = null!;
        private IAluguelService aluguelService = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var builder = new DbContextOptionsBuilder<AluguelinkContext>();
            builder.UseInMemoryDatabase("aluguelinkdb_aluguel");
            var options = builder.Options;

            context = new AluguelinkContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Setup básico para testes futuros
            aluguelService = new Service.AluguelService(context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // TODO: Implementar teste de criação de aluguel
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void GetTest()
        {
            // TODO: Implementar teste de busca de aluguel
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // TODO: Implementar teste de listagem de aluguels
            Assert.IsTrue(true);
        }
    }
}