using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass()]
    public class ImovelServiceTests
    {
        private AluguelinkContext context = null!;
        private IImovelService imovelService = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var builder = new DbContextOptionsBuilder<AluguelinkContext>();
            builder.UseInMemoryDatabase("aluguelinkdb_imovel");
            var options = builder.Options;

            context = new AluguelinkContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var locadores = new List<Locador>
            {
                new() {
                    Id = 1,
                    Nome = "Pedro Proprietário",
                    Email = "pedro@gmail.com",
                    Cpf = "11111111111",
                    Telefone = "11999999999"
                },
                new() {
                    Id = 2,
                    Nome = "Ana Proprietária",
                    Email = "ana@gmail.com",
                    Cpf = "22222222222",
                    Telefone = "21999999999"
                }
            };

            var imoveis = new List<Imovel>
            {
                new() {
                    Id = 1,
                    Cep = "01234567",
                    Logradouro = "Rua das Flores",
                    Numero = "123",
                    Complemento = "Apto 45",
                    Bairro = "Vila Madalena",
                    Cidade = "São Paulo",
                    Estado = "SP",
                    Tipo = "A", // Apartamento
                    Quartos = 3,
                    Banheiros = 2,
                    Area = 120.50m,
                    VagasGaragem = 2,
                    Valor = 3500.00m,
                    Descricao = "Apartamento amplo e bem localizado",
                    IdLocador = 1
                },
                new() {
                    Id = 2,
                    Cep = "20000000",
                    Logradouro = "Av. Copacabana",
                    Numero = "1000",
                    Bairro = "Copacabana",
                    Cidade = "Rio de Janeiro",
                    Estado = "RJ",
                    Tipo = "C", // Casa
                    Quartos = 4,
                    Banheiros = 3,
                    Area = 200.00m,
                    VagasGaragem = 3,
                    Valor = 5000.00m,
                    Descricao = "Casa espaçosa na praia",
                    IdLocador = 2
                },
                new() {
                    Id = 3,
                    Cep = "30000000",
                    Logradouro = "Rua da Bahia",
                    Numero = "500",
                    Bairro = "Centro",
                    Cidade = "Belo Horizonte",
                    Estado = "MG",
                    Tipo = "PC", // Ponto Comercial
                    Quartos = 0,
                    Banheiros = 2,
                    Area = 80.00m,
                    VagasGaragem = 1,
                    Valor = 2500.00m,
                    Descricao = "Loja comercial no centro",
                    IdLocador = 1
                },
                new() {
                    Id = 4,
                    Cep = "80000000",
                    Logradouro = "Av. Presidente",
                    Numero = "200",
                    Bairro = "Centro",
                    Cidade = "Curitiba",
                    Estado = "PR",
                    Tipo = "A", // Apartamento
                    Quartos = 2,
                    Banheiros = 1,
                    Area = 85.00m,
                    VagasGaragem = 1,
                    Valor = 2200.00m,
                    Descricao = "Apartamento compacto",
                    IdLocador = 2
                }
            };

            context.AddRange(locadores);
            context.AddRange(imoveis);
            context.SaveChanges();

            imovelService = new Service.ImovelService(context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            var imovel = new Imovel
            {
                Cep = "90000000",
                Logradouro = "Rua Porto Alegre",
                Numero = "300",
                Bairro = "Moinhos",
                Cidade = "Porto Alegre",
                Estado = "RS",
                Tipo = "C", // Casa
                Quartos = 3,
                Banheiros = 2,
                Area = 150.00m,
                VagasGaragem = 2,
                Valor = 4000.00m,
                Descricao = "Casa moderna",
                IdLocador = 1
            };

            imovelService.Create(imovel);

            // Assert
            Assert.AreEqual(5, imovelService.GetAll(page, pageSize).Count());
            var imovelInserido = imovelService.Get(imovel.Id);
            Assert.IsNotNull(imovelInserido);
            Assert.AreEqual("Rua Porto Alegre", imovelInserido.Logradouro);
            Assert.AreEqual("Porto Alegre", imovelInserido.Cidade);
            Assert.AreEqual("RS", imovelInserido.Estado);
            Assert.AreEqual("C", imovelInserido.Tipo);
            Assert.AreEqual(4000.00m, imovelInserido.Valor);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Act
            imovelService.Delete(2);

            // Assert
            Assert.AreEqual(3, imovelService.GetAll(page, pageSize).Count());
            var imovel = imovelService.Get(2);
            Assert.IsNull(imovel);
        }

        [TestMethod()]
        public void EditTest()
        {
            //Act 
            var imovel = imovelService.Get(3);
            Assert.IsNotNull(imovel);
            imovel.Logradouro = "Rua Editada";
            imovel.Numero = "999";
            imovel.Valor = 3000.00m;
            imovel.Descricao = "Descrição editada";
            imovel.Quartos = 1;
            imovel.Banheiros = 1;
            imovelService.Edit(imovel);

            //Assert
            imovel = imovelService.Get(3);
            Assert.IsNotNull(imovel);
            Assert.AreEqual("Rua Editada", imovel.Logradouro);
            Assert.AreEqual("999", imovel.Numero);
            Assert.AreEqual(3000.00m, imovel.Valor);
            Assert.AreEqual("Descrição editada", imovel.Descricao);
            Assert.AreEqual(1, imovel.Quartos);
            Assert.AreEqual(1, imovel.Banheiros);
        }

        [TestMethod()]
        public void GetTest()
        {
            // Act
            var imovel = imovelService.Get(1);

            // Assert
            Assert.IsNotNull(imovel);
            Assert.AreEqual("Rua das Flores", imovel.Logradouro);
            Assert.AreEqual("São Paulo", imovel.Cidade);
            Assert.AreEqual("SP", imovel.Estado);
            Assert.AreEqual("A", imovel.Tipo);
            Assert.AreEqual(3, imovel.Quartos);
            Assert.AreEqual(2, imovel.Banheiros);
            Assert.AreEqual(3500.00m, imovel.Valor);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaImoveis = imovelService.GetAll(page, pageSize);

            // Assert
            Assert.IsInstanceOfType(listaImoveis, typeof(IEnumerable<Imovel>));
            Assert.IsNotNull(listaImoveis);
            Assert.AreEqual(4, listaImoveis.Count());
            Assert.AreEqual(1, listaImoveis.First().Id);
            Assert.AreEqual("Rua das Flores", listaImoveis.First().Logradouro);
        }

        [TestMethod()]
        public void GetByLocadorTest()
        {
            //Act
            var imoveis = imovelService.GetByLocador(1);

            //Assert
            Assert.IsInstanceOfType(imoveis, typeof(IEnumerable<Core.DTO.ImovelDTO>));
            Assert.IsNotNull(imoveis);
            Assert.AreEqual(2, imoveis.Count());
            Assert.IsTrue(imoveis.Any(i => i.Logradouro == "Rua das Flores"));
            Assert.IsTrue(imoveis.Any(i => i.Logradouro == "Rua da Bahia"));
        }

        [TestMethod()]
        public void GetByTipoTest()
        {
            //Act
            var imoveis = imovelService.GetByTipo("A");

            //Assert
            Assert.IsInstanceOfType(imoveis, typeof(IEnumerable<Core.DTO.ImovelDTO>));
            Assert.IsNotNull(imoveis);
            Assert.AreEqual(2, imoveis.Count());
            Assert.IsTrue(imoveis.All(i => i.Tipo == "A"));
        }

        [TestMethod()]
        public void GetByValorRangeTest()
        {
            //Act
            var imoveis = imovelService.GetByValorRange(2000.00m, 3000.00m);

            //Assert
            Assert.IsInstanceOfType(imoveis, typeof(IEnumerable<Core.DTO.ImovelDTO>));
            Assert.IsNotNull(imoveis);
            Assert.AreEqual(2, imoveis.Count());
            Assert.IsTrue(imoveis.All(i => i.Valor >= 2000.00m && i.Valor <= 3000.00m));
        }

        [TestMethod()]
        public void GetCountTest()
        {
            // Act
            var count = imovelService.GetCount();

            // Assert
            Assert.AreEqual(4, count);
        }
    }
}