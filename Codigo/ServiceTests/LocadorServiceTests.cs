using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass]
    public class LocadorServiceTests
    {
        private AluguelinkContext context = null!;
        private ILocadorService locadorService = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new DbContextOptionsBuilder<AluguelinkContext>()
                .UseInMemoryDatabase("aluguelinkdb");
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
                },
                new() {
                    Id = 3,
                    Nome = "Carlos Silva",
                    Email = "carlos@gmail.com",
                    Cpf = "11122233344",
                    Telefone = "11888888888"
                }
            };

            context.AddRange(locadores);
            context.SaveChanges();

            locadorService = new Service.LocadorService(context);
        }

        [TestMethod]
        public void CreateTest()
        {
            var novoLocador = locadorService.Create(new Locador()
            {
                Nome = "Ana Proprietária",
                Email = "ana@gmail.com",
                Cpf = "55566677788",
                Telefone = "31999999999"
            });

            Assert.AreEqual(4, novoLocador);
            Assert.AreEqual(4, locadorService.GetAll(page, pageSize).Count());
            var locador = locadorService.Get(4);
            Assert.IsNotNull(locador);
            Assert.AreEqual(4, locador.Id);
            Assert.AreEqual("Ana Proprietária", locador.Nome);
            Assert.AreEqual("ana@gmail.com", locador.Email);
            Assert.AreEqual("55566677788", locador.Cpf);
            Assert.AreEqual("31999999999", locador.Telefone);
        }

        [TestMethod]
        public void DeleteTest()
        {
            locadorService.Delete(2);

            Assert.AreEqual(2, locadorService.GetAll(page, pageSize).Count());
            var locador = locadorService.Get(2);
            Assert.IsNull(locador);
        }

        [TestMethod]
        public void EditTest()
        {
            var locador = locadorService.Get(3);
            Assert.IsNotNull(locador);
            locador.Nome = "Carlos Silva Editado";
            locador.Email = "carlos.editado@gmail.com";
            locador.Telefone = "11777777777";
            locadorService.Edit(locador);

            locador = locadorService.Get(3);
            Assert.IsNotNull(locador);
            Assert.AreEqual("Carlos Silva Editado", locador.Nome);
            Assert.AreEqual("carlos.editado@gmail.com", locador.Email);
            Assert.AreEqual("11777777777", locador.Telefone);
            Assert.AreEqual(3, locador.Id);
        }

        [TestMethod]
        public void GetTest()
        {
            var locador = locadorService.Get(1);

            Assert.IsNotNull(locador);
            Assert.AreEqual("João Proprietário", locador.Nome);
            Assert.AreEqual("joao@gmail.com", locador.Email);
            Assert.AreEqual("12345678901", locador.Cpf);
            Assert.AreEqual("11999999999", locador.Telefone);
            Assert.AreEqual(1, locador.Id);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var listaLocadores = locadorService.GetAll(page, pageSize);

            Assert.IsInstanceOfType(listaLocadores, typeof(IEnumerable<Locador>));
            Assert.IsNotNull(listaLocadores);
            Assert.AreEqual(3, listaLocadores.Count());
            Assert.AreEqual(1, listaLocadores.First().Id);
            Assert.AreEqual("João Proprietário", listaLocadores.First().Nome);
        }

        [TestMethod]
        public void GetByCpfTest()
        {
            var locadores = locadorService.GetByCpf("12345678901");

            Assert.IsInstanceOfType(locadores, typeof(IEnumerable<LocadorDto>));
            Assert.IsNotNull(locadores);
            Assert.AreEqual(1, locadores.Count());
            var locador = locadores.First();
            Assert.AreEqual("João Proprietário", locador.Nome);
            Assert.AreEqual("12345678901", locador.Cpf);
            Assert.AreEqual("joao@gmail.com", locador.Email);
        }

        [TestMethod]
        public void GetByCpfTest_Inexistente_DeveRetornarVazio()
        {
            var locadores = locadorService.GetByCpf("00000000000");

            Assert.IsInstanceOfType(locadores, typeof(IEnumerable<LocadorDto>));
            Assert.IsNotNull(locadores);
            Assert.AreEqual(0, locadores.Count());
        }

        [TestMethod]
        public void GetByNomeTest()
        {
            var locadores = locadorService.GetByNome("Maria");

            Assert.IsInstanceOfType(locadores, typeof(IEnumerable<LocadorDto>));
            Assert.IsNotNull(locadores);
            Assert.AreEqual(1, locadores.Count());
            var locador = locadores.First();
            Assert.AreEqual("Maria Proprietária", locador.Nome);
            Assert.AreEqual("maria@gmail.com", locador.Email);
        }

        [TestMethod]
        public void GetByNomeTest_BuscaParcial()
        {
            var locadores = locadorService.GetByNome("Silva");

            Assert.IsInstanceOfType(locadores, typeof(IEnumerable<LocadorDto>));
            Assert.IsNotNull(locadores);
            Assert.AreEqual(1, locadores.Count());
            var locador = locadores.First();
            Assert.AreEqual("Carlos Silva", locador.Nome);
        }

        [TestMethod]
        public void GetCountTest()
        {
            var count = locadorService.GetCount();

            Assert.AreEqual(3, count);
        }
    }
}