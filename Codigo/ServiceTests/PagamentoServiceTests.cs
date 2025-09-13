using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass]
    public class PagamentoServiceTests
    {
        private AluguelinkContext context = null!;
        private IPagamentoService pagamentoService = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new DbContextOptionsBuilder<AluguelinkContext>()
                .UseInMemoryDatabase("pagamentodb");
            var options = builder.Options;

            context = new AluguelinkContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var locadores = new List<Locador>
            {
                new() { Id = 1, Nome = "João Locador", Email = "joao@locador.com", Cpf = "12345678901", Telefone = "11999999999" }
            };

            var locatarios = new List<Locatario>
            {
                new() {
                    Id = 1, Nome = "Maria Silva", Email = "maria@gmail.com", Cpf = "98765432100",
                    Telefone1 = "11888888888", Telefone2 = "11777777777", Cep = "01234567",
                    Logradouro = "Rua A", Numero = "100", Bairro = "Centro", Cidade = "São Paulo", Estado = "SP"
                },
                new() {
                    Id = 2, Nome = "Carlos Santos", Email = "carlos@gmail.com", Cpf = "11122233344",
                    Telefone1 = "11666666666", Telefone2 = "11555555555", Cep = "02345678",
                    Logradouro = "Rua B", Numero = "200", Bairro = "Vila Nova", Cidade = "São Paulo", Estado = "SP"
                }
            };

            var imoveis = new List<Imovel>
            {
                new() {
                    Id = 1, IdLocador = 1, Cep = "01234567", Logradouro = "Rua A", Numero = "100",
                    Bairro = "Centro", Cidade = "São Paulo", Estado = "SP", Tipo = "A",
                    Quartos = 2, Banheiros = 1, Area = 80.0m, VagasGaragem = 1, Valor = 2000.00m,
                    Descricao = "Apartamento teste"
                },
                new() {
                    Id = 2, IdLocador = 1, Cep = "02345678", Logradouro = "Rua B", Numero = "200",
                    Bairro = "Vila Nova", Cidade = "São Paulo", Estado = "SP", Tipo = "C",
                    Quartos = 3, Banheiros = 2, Area = 120.0m, VagasGaragem = 2, Valor = 3000.00m,
                    Descricao = "Casa teste"
                }
            };

            var alugueis = new List<Aluguel>
            {
                new() {
                    Id = 1, Idlocatario = 1, Idimovel = 1,
                    DataInicio = DateOnly.FromDateTime(DateTime.Now.AddMonths(-2)),
                    DataFim = DateOnly.FromDateTime(DateTime.Now.AddMonths(10)),
                    DataAssinatura = DateOnly.FromDateTime(DateTime.Now.AddMonths(-2)),
                    Status = "A"
                },
                new() {
                    Id = 2, Idlocatario = 2, Idimovel = 2,
                    DataInicio = DateOnly.FromDateTime(DateTime.Now.AddMonths(-12)),
                    DataFim = DateOnly.FromDateTime(DateTime.Now.AddMonths(-2)),
                    DataAssinatura = DateOnly.FromDateTime(DateTime.Now.AddMonths(-12)),
                    Status = "F"
                }
            };

            var pagamentos = new List<Pagamento>
            {
                new() {
                    Id = 1, Valor = 2000.00m, DataPagamento = DateTime.Now.AddDays(-30),
                    TipoPagamento = "P", Idaluguel = 1
                },
                new() {
                    Id = 2, Valor = 3000.00m, DataPagamento = DateTime.Now.AddDays(-15),
                    TipoPagamento = "CC", Idaluguel = 2
                },
                new() {
                    Id = 3, Valor = 2000.00m, DataPagamento = DateTime.Now.AddDays(-5),
                    TipoPagamento = "B", Idaluguel = 1
                }
            };

            context.AddRange(locadores);
            context.AddRange(locatarios);
            context.AddRange(imoveis);
            context.AddRange(alugueis);
            context.AddRange(pagamentos);
            context.SaveChanges();

            pagamentoService = new Service.PagamentoService(context);
        }

        [TestMethod]
        public void CreateTest()
        {
            var novoPagamento = pagamentoService.Create(new Pagamento()
            {
                Valor = 1500.00m,
                DataPagamento = DateTime.Now,
                TipoPagamento = "CD",
                Idaluguel = 1
            });

            Assert.AreEqual(4, novoPagamento);
            Assert.AreEqual(4, pagamentoService.GetAll(page, pageSize).Count());
            var pagamento = pagamentoService.Get(4);
            Assert.IsNotNull(pagamento);
            Assert.AreEqual(4, pagamento.Id);
            Assert.AreEqual(1500.00m, pagamento.Valor);
            Assert.AreEqual("CD", pagamento.TipoPagamento);
            Assert.AreEqual(1, pagamento.Idaluguel);
        }

        [TestMethod]
        public void DeleteTest()
        {
            pagamentoService.Delete(2);

            Assert.AreEqual(2, pagamentoService.GetAll(page, pageSize).Count());
            var pagamento = pagamentoService.Get(2);
            Assert.IsNull(pagamento);
        }

        [TestMethod]
        public void EditTest()
        {
            var pagamento = pagamentoService.Get(3);
            Assert.IsNotNull(pagamento);
            pagamento.Valor = 2500.00m;
            pagamento.TipoPagamento = "P";
            pagamentoService.Edit(pagamento);

            pagamento = pagamentoService.Get(3);
            Assert.IsNotNull(pagamento);
            Assert.AreEqual(2500.00m, pagamento.Valor);
            Assert.AreEqual("P", pagamento.TipoPagamento);
            Assert.AreEqual(3, pagamento.Id);
        }

        [TestMethod]
        public void GetTest()
        {
            var pagamento = pagamentoService.Get(1);

            Assert.IsNotNull(pagamento);
            Assert.AreEqual(2000.00m, pagamento.Valor);
            Assert.AreEqual("P", pagamento.TipoPagamento);
            Assert.AreEqual(1, pagamento.Idaluguel);
            Assert.AreEqual(1, pagamento.Id);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var listaPagamentos = pagamentoService.GetAll(page, pageSize);

            Assert.IsInstanceOfType(listaPagamentos, typeof(IEnumerable<Pagamento>));
            Assert.IsNotNull(listaPagamentos);
            Assert.AreEqual(3, listaPagamentos.Count());
            Assert.AreEqual(1, listaPagamentos.First().Id);
            Assert.AreEqual(2000.00m, listaPagamentos.First().Valor);
        }

        [TestMethod]
        public void GetByAluguelTest()
        {
            var pagamentos = pagamentoService.GetByAluguel(1);

            Assert.IsInstanceOfType(pagamentos, typeof(IEnumerable<PagamentoDto>));
            Assert.IsNotNull(pagamentos);
            Assert.AreEqual(2, pagamentos.Count());
            var primeiroPagamento = pagamentos.First();
            Assert.AreEqual(1, primeiroPagamento.Idaluguel);
        }

        [TestMethod]
        public void GetByTipoPagamentoTest()
        {
            var pagamentos = pagamentoService.GetByTipoPagamento("P");

            Assert.IsInstanceOfType(pagamentos, typeof(IEnumerable<PagamentoDto>));
            Assert.IsNotNull(pagamentos);
            Assert.AreEqual(1, pagamentos.Count());
            var pagamento = pagamentos.First();
            Assert.AreEqual("P", pagamento.TipoPagamento);
        }

        [TestMethod]
        public void GetByPeriodoTest()
        {
            var dataInicio = DateTime.Now.AddDays(-20);
            var dataFim = DateTime.Now.AddDays(-10);
            var pagamentos = pagamentoService.GetByPeriodo(dataInicio, dataFim);

            Assert.IsInstanceOfType(pagamentos, typeof(IEnumerable<PagamentoDto>));
            Assert.IsNotNull(pagamentos);
            Assert.AreEqual(1, pagamentos.Count());
            var pagamento = pagamentos.First();
            Assert.IsTrue(pagamento.DataPagamento >= dataInicio && pagamento.DataPagamento <= dataFim);
        }

        [TestMethod]
        public void GetByLocadorTest()
        {
            var pagamentos = pagamentoService.GetByLocador(1, page, pageSize);

            Assert.IsInstanceOfType(pagamentos, typeof(IEnumerable<Pagamento>));
            Assert.IsNotNull(pagamentos);
            Assert.AreEqual(3, pagamentos.Count());
        }

        [TestMethod]
        public void GetCountByLocadorTest()
        {
            var count = pagamentoService.GetCountByLocador(1);

            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void GetCountTest()
        {
            var count = pagamentoService.GetCount();

            Assert.AreEqual(3, count);
        }
    }
}