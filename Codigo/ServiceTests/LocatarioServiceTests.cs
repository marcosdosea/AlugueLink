using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass()]
    public class LocatarioServiceTests
    {
        private AluguelinkContext context = null!;
        private ILocatarioService locatarioService = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var builder = new DbContextOptionsBuilder<AluguelinkContext>();
            builder.UseInMemoryDatabase("aluguelinkdb");
            var options = builder.Options;

            context = new AluguelinkContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var locatarios = new List<Locatario>
            {
                new() {
                    Id = 1,
                    Nome = "João Silva",
                    Email = "joao@gmail.com",
                    Telefone1 = "11999999999",
                    Telefone2 = "1133445566",
                    Cpf = "12345678901",
                    Cep = "01234567",
                    Logradouro = "Rua das Flores",
                    Numero = "123",
                    Complemento = "Apto 45",
                    Bairro = "Vila Madalena",
                    Cidade = "São Paulo",
                    Estado = "SP",
                    Profissao = "Engenheiro",
                    Renda = 5000.50m
                },
                new() {
                    Id = 2,
                    Nome = "Maria Santos",
                    Email = "maria@gmail.com",
                    Telefone1 = "21987654321",
                    Telefone2 = "2133445566",
                    Cpf = "98765432100",
                    Cep = "20000000",
                    Logradouro = "Av. Copacabana",
                    Numero = "1000",
                    Bairro = "Copacabana",
                    Cidade = "Rio de Janeiro",
                    Estado = "RJ",
                    Profissao = "Médica",
                    Renda = 8000.00m
                },
                new() {
                    Id = 3,
                    Nome = "Carlos Pereira",
                    Email = "carlos@gmail.com",
                    Telefone1 = "31987654321",
                    Telefone2 = "3133445566",
                    Cpf = "11122233344",
                    Cep = "30000000",
                    Logradouro = "Rua da Bahia",
                    Numero = "500",
                    Bairro = "Centro",
                    Cidade = "Belo Horizonte",
                    Estado = "MG",
                    Profissao = "Advogado",
                    Renda = 6500.00m
                },
                new() {
                    Id = 4,
                    Nome = "Ana Costa",
                    Email = "ana@gmail.com",
                    Telefone1 = "41987654321",
                    Telefone2 = "4133445566",
                    Cpf = "55566677788",
                    Cep = "80000000",
                    Logradouro = "Av. Presidente",
                    Numero = "200",
                    Bairro = "Centro",
                    Cidade = "Curitiba",
                    Estado = "PR",
                    Profissao = "Professora",
                    Renda = 4500.00m
                }
            };

            context.AddRange(locatarios);
            context.SaveChanges();

            locatarioService = new Service.LocatarioService(context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            var locatario = new Locatario
            {
                Nome = "Bruno Lima",
                Email = "bruno@gmail.com",
                Telefone1 = "51987654321",
                Telefone2 = "5133445566",
                Cpf = "99988877766",
                Cep = "90000000",
                Logradouro = "Rua Porto Alegre",
                Numero = "300",
                Bairro = "Moinhos",
                Cidade = "Porto Alegre",
                Estado = "RS",
                Profissao = "Contador",
                Renda = 5500.00m
            };

            locatarioService.Create(locatario);

            // Assert
            Assert.AreEqual(5, locatarioService.GetAll(page, pageSize).Count());
            var locatarioInserido = locatarioService.Get(locatario.Id);
            Assert.IsNotNull(locatarioInserido);
            Assert.AreEqual("Bruno Lima", locatarioInserido.Nome);
            Assert.AreEqual("bruno@gmail.com", locatarioInserido.Email);
            Assert.AreEqual("99988877766", locatarioInserido.Cpf);
            Assert.AreEqual("Porto Alegre", locatarioInserido.Cidade);
            Assert.AreEqual("RS", locatarioInserido.Estado);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Act
            locatarioService.Delete(2);

            // Assert
            Assert.AreEqual(3, locatarioService.GetAll(page, pageSize).Count());
            var locatario = locatarioService.Get(2);
            Assert.IsNull(locatario);
        }

        [TestMethod()]
        public void EditTest()
        {
            //Act 
            var locatario = locatarioService.Get(3);
            Assert.IsNotNull(locatario);
            locatario.Nome = "Carlos Silva Editado";
            locatario.Email = "carlos.editado@gmail.com";
            locatario.Profissao = "Contador";
            locatario.Renda = 7000.00m;
            locatario.Cidade = "Nova Lima";
            locatarioService.Edit(locatario);

            //Assert
            locatario = locatarioService.Get(3);
            Assert.IsNotNull(locatario);
            Assert.AreEqual("Carlos Silva Editado", locatario.Nome);
            Assert.AreEqual("carlos.editado@gmail.com", locatario.Email);
            Assert.AreEqual("Contador", locatario.Profissao);
            Assert.AreEqual(7000.00m, locatario.Renda);
            Assert.AreEqual("Nova Lima", locatario.Cidade);
        }

        [TestMethod()]
        public void GetTest()
        {
            // Act
            var locatario = locatarioService.Get(1);

            // Assert
            Assert.IsNotNull(locatario);
            Assert.AreEqual("João Silva", locatario.Nome);
            Assert.AreEqual("joao@gmail.com", locatario.Email);
            Assert.AreEqual("12345678901", locatario.Cpf);
            Assert.AreEqual("São Paulo", locatario.Cidade);
            Assert.AreEqual("SP", locatario.Estado);
            Assert.AreEqual("Engenheiro", locatario.Profissao);
            Assert.AreEqual(5000.50m, locatario.Renda);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaLocatarios = locatarioService.GetAll(page, pageSize);

            // Assert
            Assert.IsInstanceOfType(listaLocatarios, typeof(IEnumerable<Locatario>));
            Assert.IsNotNull(listaLocatarios);
            Assert.AreEqual(4, listaLocatarios.Count());
            Assert.AreEqual(1, listaLocatarios.First().Id);
            Assert.AreEqual("João Silva", listaLocatarios.First().Nome);
        }

        [TestMethod()]
        public void GetByCpfTest()
        {
            //Act
            var locatarios = locatarioService.GetByCpf("12345678901");

            //Assert
            Assert.IsInstanceOfType(locatarios, typeof(IEnumerable<Core.DTO.LocatarioDto>));
            Assert.IsNotNull(locatarios);
            Assert.AreEqual(1, locatarios.Count());
            var locatario = locatarios.First();
            Assert.AreEqual("João Silva", locatario.Nome);
            Assert.AreEqual("12345678901", locatario.Cpf);
        }

        [TestMethod()]
        public void GetByNomeTest()
        {
            //Act
            var locatarios = locatarioService.GetByNome("Maria");

            //Assert
            Assert.IsInstanceOfType(locatarios, typeof(IEnumerable<Core.DTO.LocatarioDto>));
            Assert.IsNotNull(locatarios);
            Assert.AreEqual(1, locatarios.Count());
            var locatario = locatarios.First();
            Assert.AreEqual("Maria Santos", locatario.Nome);
            Assert.AreEqual("maria@gmail.com", locatario.Email);
        }

        [TestMethod()]
        public void GetCountTest()
        {
            // Act
            var count = locatarioService.GetCount();

            // Assert
            Assert.AreEqual(4, count);
        }
    }
}