using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass]
    public class AluguelServiceTests
    {
        private AluguelinkContext context = null!;
        private IAluguelService aluguelService = null!;
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
                new() { Id = 1, Nome = "João", Email = "joao@gmail.com", Cpf = "12345678901", Telefone = "11999999999" }
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
                },
                new() {
                    Id = 3, Idlocatario = 2, Idimovel = 1,
                    DataInicio = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),
                    DataFim = DateOnly.FromDateTime(DateTime.Now.AddMonths(13)),
                    DataAssinatura = DateOnly.FromDateTime(DateTime.Now),
                    Status = "P"
                }
            };

            context.AddRange(locadores);
            context.AddRange(locatarios);
            context.AddRange(imoveis);
            context.AddRange(alugueis);
            context.SaveChanges();

            aluguelService = new Service.AluguelService(context);
        }

        [TestMethod]
        public void CreateTest()
        {
            var novoAluguel = aluguelService.Create(new Aluguel()
            {
                Idlocatario = 1,
                Idimovel = 2,
                DataInicio = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                DataFim = DateOnly.FromDateTime(DateTime.Now.AddDays(395)),
                DataAssinatura = DateOnly.FromDateTime(DateTime.Now),
                Status = "P"
            });

            Assert.AreEqual(4, novoAluguel);
            Assert.AreEqual(4, aluguelService.GetAll(page, pageSize).Count());
            var aluguel = aluguelService.Get(4);
            Assert.IsNotNull(aluguel);
            Assert.AreEqual(4, aluguel.Id);
            Assert.AreEqual(1, aluguel.Idlocatario);
            Assert.AreEqual(2, aluguel.Idimovel);
            Assert.AreEqual("P", aluguel.Status);
        }

        [TestMethod]
        public void DeleteTest()
        {
            aluguelService.Delete(2);

            Assert.AreEqual(2, aluguelService.GetAll(page, pageSize).Count());
            var aluguel = aluguelService.Get(2);
            Assert.IsNull(aluguel);
        }

        [TestMethod]
        public void EditTest()
        {
            var aluguel = aluguelService.Get(3);
            Assert.IsNotNull(aluguel);
            aluguel.Status = "A"; 
            aluguel.DataInicio = DateOnly.FromDateTime(DateTime.Now);
            aluguelService.Edit(aluguel);

            aluguel = aluguelService.Get(3);
            Assert.IsNotNull(aluguel);
            Assert.AreEqual("A", aluguel.Status);
            Assert.AreEqual(DateOnly.FromDateTime(DateTime.Now), aluguel.DataInicio);
            Assert.AreEqual(3, aluguel.Id);
        }

        [TestMethod]
        public void GetTest()
        {
            var aluguel = aluguelService.Get(1);

            Assert.IsNotNull(aluguel);
            Assert.AreEqual(1, aluguel.Idlocatario);
            Assert.AreEqual(1, aluguel.Idimovel);
            Assert.AreEqual("A", aluguel.Status);
            Assert.AreEqual(1, aluguel.Id);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var listaAlugueis = aluguelService.GetAll(page, pageSize);

            Assert.IsInstanceOfType(listaAlugueis, typeof(IEnumerable<Aluguel>));
            Assert.IsNotNull(listaAlugueis);
            Assert.AreEqual(3, listaAlugueis.Count());
            Assert.AreEqual(1, listaAlugueis.First().Id);
            Assert.AreEqual("A", listaAlugueis.First().Status);
        }

        [TestMethod]
        public void GetByLocatarioTest()
        {
            var alugueis = aluguelService.GetByLocatario(1);

            Assert.IsInstanceOfType(alugueis, typeof(IEnumerable<AluguelDto>));
            Assert.IsNotNull(alugueis);
            Assert.AreEqual(1, alugueis.Count());
            var aluguel = alugueis.First();
            Assert.AreEqual(1, aluguel.Idlocatario);
            Assert.AreEqual("A", aluguel.Status);
        }

        [TestMethod]
        public void GetByImovelTest()
        {
            var alugueis = aluguelService.GetByImovel(1);

            Assert.IsInstanceOfType(alugueis, typeof(IEnumerable<AluguelDto>));
            Assert.IsNotNull(alugueis);
            Assert.AreEqual(2, alugueis.Count());
            Assert.IsTrue(alugueis.All(a => a.Idimovel == 1));
        }

        [TestMethod]
        public void GetByStatusTest()
        {
            var alugueis = aluguelService.GetByStatus("A");

            Assert.IsInstanceOfType(alugueis, typeof(IEnumerable<AluguelDto>));
            Assert.IsNotNull(alugueis);
            Assert.AreEqual(1, alugueis.Count());
            var aluguel = alugueis.First();
            Assert.AreEqual("A", aluguel.Status);
        }

        [TestMethod]
        public void IsImovelAvailableTest_ImovelDisponivel()
        {
            var disponivel = aluguelService.IsImovelAvailable(2);

            Assert.IsTrue(disponivel);
        }

        [TestMethod]
        public void IsImovelAvailableTest_ImovelOcupado()
        {
            var disponivel = aluguelService.IsImovelAvailable(1);

            Assert.IsFalse(disponivel);
        }

        [TestMethod]
        public void IsLocatarioAvailableTest_LocatarioDisponivel()
        {
            var disponivel = aluguelService.IsLocatarioAvailable(2);

            Assert.IsTrue(disponivel);
        }

        [TestMethod]
        public void IsLocatarioAvailableTest_LocatarioOcupado()
        {
            var disponivel = aluguelService.IsLocatarioAvailable(1);

            Assert.IsFalse(disponivel);
        }

        [TestMethod]
        public void GetImoveisIndisponiveisTest()
        {
            var imoveisOcupados = aluguelService.GetImoveisIndisponiveis();

            Assert.IsInstanceOfType(imoveisOcupados, typeof(IEnumerable<int>));
            Assert.IsNotNull(imoveisOcupados);
            Assert.AreEqual(1, imoveisOcupados.Count());
            Assert.IsTrue(imoveisOcupados.Contains(1));
        }

        [TestMethod]
        public void GetLocatariosOcupadosTest()
        {
            var locatariosOcupados = aluguelService.GetLocatariosOcupados();

            Assert.IsInstanceOfType(locatariosOcupados, typeof(IEnumerable<int>));
            Assert.IsNotNull(locatariosOcupados);
            Assert.AreEqual(1, locatariosOcupados.Count());
            Assert.IsTrue(locatariosOcupados.Contains(1));
        }

        [TestMethod]
        public void GetAluguelAtivoByImovelTest()
        {
            var aluguelAtivo = aluguelService.GetAluguelAtivoByImovel(1);

            Assert.IsNotNull(aluguelAtivo);
            Assert.AreEqual(1, aluguelAtivo.Id);
            Assert.AreEqual("A", aluguelAtivo.Status);
        }

        [TestMethod]
        public void GetAluguelAtivoByLocatarioTest()
        {
            var aluguelAtivo = aluguelService.GetAluguelAtivoByLocatario(1);

            Assert.IsNotNull(aluguelAtivo);
            Assert.AreEqual(1, aluguelAtivo.Id);
            Assert.AreEqual("A", aluguelAtivo.Status);
        }

        [TestMethod]
        public void GetCountTest()
        {
            var count = aluguelService.GetCount();

            Assert.AreEqual(3, count);
        }

        #region Testes de Integração - Técnicas de Caixa Branca

        #region Helpers para Setup de Testes

        /// <summary>
        /// Cria um contexto InMemory isolado com dados base para testes de loop.
        /// </summary>
        private static (AluguelinkContext context, AluguelService service) CriarContextoTeste(string dbName)
        {
            var builder = new DbContextOptionsBuilder<AluguelinkContext>()
                .UseInMemoryDatabase(dbName);
            var context = new AluguelinkContext(builder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Locadors.Add(new Locador 
            { 
                Id = 1, Nome = "Test", Email = "t@t.com", Cpf = "12345678901", Telefone = "123" 
            });
            context.Locatarios.Add(new Locatario
            {
                Id = 1, Nome = "Test", Email = "t@t.com", Cpf = "12345678901",
                Telefone1 = "123", Telefone2 = "456", Cep = "12345678", Logradouro = "Rua",
                Numero = "1", Bairro = "Centro", Cidade = "SP", Estado = "SP"
            });

            return (context, new AluguelService(context));
        }

        private static Imovel CriarImovel(int id, string descricao = "Imóvel teste") => new()
        {
            Id = id, IdLocador = 1, Cep = "12345678", Logradouro = $"Rua {id}",
            Numero = id.ToString(), Bairro = "Centro", Cidade = "SP", Estado = "SP",
            Tipo = "A", Quartos = 1, Banheiros = 1, Area = 50m, VagasGaragem = 1, 
            Valor = 1000m, Descricao = descricao
        };

        #endregion

        #region Teste de Ciclo (Loop Testing) - AtualizarStatusAlugueis

        [TestMethod]
        public void AtualizarStatusAlugueis_ZeroIteracoes_StatusPermanece()
        {
            var (context, service) = CriarContextoTeste("db_loop_zero");
            context.Imovels.Add(CriarImovel(1));
            context.Aluguels.Add(new Aluguel
            {
                Id = 1, Idlocatario = 1, Idimovel = 1, Status = "A",
                DataInicio = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)),
                DataFim = DateOnly.FromDateTime(DateTime.Now.AddMonths(11)),
                DataAssinatura = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1))
            });
            context.SaveChanges();

            service.AtualizarStatusAlugueis();

            Assert.AreEqual("A", context.Aluguels.Find(1)?.Status);
        }

        [TestMethod]
        public void AtualizarStatusAlugueis_UmaIteracao_StatusMudaParaFinalizado()
        {
            var (context, service) = CriarContextoTeste("db_loop_one");
            context.Imovels.Add(CriarImovel(1));
            context.Aluguels.Add(new Aluguel
            {
                Id = 1, Idlocatario = 1, Idimovel = 1, Status = "A",
                DataInicio = DateOnly.FromDateTime(DateTime.Now.AddMonths(-12)),
                DataFim = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                DataAssinatura = DateOnly.FromDateTime(DateTime.Now.AddMonths(-12))
            });
            context.SaveChanges();

            service.AtualizarStatusAlugueis();

            Assert.AreEqual("F", context.Aluguels.Find(1)?.Status);
        }

        [TestMethod]
        public void AtualizarStatusAlugueis_MultiplasIteracoes_StatusesAtualizadosCorretamente()
        {
            var (context, service) = CriarContextoTeste("db_loop_multi");
            context.Imovels.AddRange(CriarImovel(1), CriarImovel(2), CriarImovel(3));
            
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            context.Aluguels.AddRange(
                new Aluguel { Id = 1, Idlocatario = 1, Idimovel = 1, Status = "A",
                    DataInicio = hoje.AddMonths(-6), DataFim = hoje.AddDays(-5), DataAssinatura = hoje.AddMonths(-6) },
                new Aluguel { Id = 2, Idlocatario = 1, Idimovel = 2, Status = "P",
                    DataInicio = hoje.AddDays(-1), DataFim = hoje.AddMonths(11), DataAssinatura = hoje.AddDays(-10) },
                new Aluguel { Id = 3, Idlocatario = 1, Idimovel = 3, Status = "A",
                    DataInicio = hoje.AddMonths(-1), DataFim = hoje.AddMonths(11), DataAssinatura = hoje.AddMonths(-1) }
            );
            context.SaveChanges();

            service.AtualizarStatusAlugueis();

            Assert.AreEqual("F", context.Aluguels.Find(1)?.Status, "Ativo expirado -> Finalizado");
            Assert.AreEqual("A", context.Aluguels.Find(2)?.Status, "Pendente iniciado -> Ativo");
            Assert.AreEqual("A", context.Aluguels.Find(3)?.Status, "Ativo válido -> Permanece");
        }

        #endregion

        #region Teste de Fluxo de Dados - NormalizeStatus

        [TestMethod]
        [DataRow("ATIVO", "A", DisplayName = "NormalizeStatus_ATIVO_RetornaA")]
        [DataRow("PENDENTE", "P", DisplayName = "NormalizeStatus_PENDENTE_RetornaP")]
        [DataRow("FINALIZADO", "F", DisplayName = "NormalizeStatus_FINALIZADO_RetornaF")]
        [DataRow("A", "A", DisplayName = "NormalizeStatus_A_PermaneceMesmo")]
        [DataRow("", "A", DisplayName = "NormalizeStatus_Vazio_DefaultA")]
        [DataRow("INVALIDO", "A", DisplayName = "NormalizeStatus_Invalido_DefaultA")]
        public void Create_NormalizaStatusCorretamente(string statusEntrada, string statusEsperado)
        {
            var novoAluguel = new Aluguel
            {
                Idlocatario = 1, Idimovel = 2,
                DataInicio = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                DataFim = DateOnly.FromDateTime(DateTime.Now.AddDays(395)),
                DataAssinatura = DateOnly.FromDateTime(DateTime.Now),
                Status = statusEntrada
            };

            var novoId = aluguelService.Create(novoAluguel);

            var aluguelCriado = aluguelService.Get(novoId);
            Assert.AreEqual(statusEsperado, aluguelCriado?.Status);
        }

        #endregion

        #region Teste de Fronteira (Boundary Testing) - IsImovelAvailable

        [TestMethod]
        public void IsImovelAvailable_PeriodoAnterior_RetornaDisponivel()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            
            var disponivel = aluguelService.IsImovelAvailable(1, hoje.AddMonths(-6), hoje.AddMonths(-3));

            Assert.IsTrue(disponivel);
        }

        [TestMethod]
        public void IsImovelAvailable_PeriodoPosterior_RetornaDisponivel()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            
            var disponivel = aluguelService.IsImovelAvailable(1, hoje.AddMonths(11), hoje.AddMonths(13));

            Assert.IsTrue(disponivel);
        }

        [TestMethod]
        public void IsImovelAvailable_PeriodoSobreposto_RetornaIndisponivel()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            
            var disponivel = aluguelService.IsImovelAvailable(1, hoje.AddMonths(-3), hoje.AddDays(1));

            Assert.IsFalse(disponivel);
        }

        [TestMethod]
        public void IsImovelAvailable_PeriodoDentro_RetornaIndisponivel()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            
            var disponivel = aluguelService.IsImovelAvailable(1, hoje, hoje.AddMonths(1));

            Assert.IsFalse(disponivel);
        }

        [TestMethod]
        public void IsImovelAvailable_SemAluguelAtivo_RetornaDisponivel()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            
            var disponivel = aluguelService.IsImovelAvailable(2, hoje, hoje.AddMonths(12));

            Assert.IsTrue(disponivel);
        }

        #endregion

        #endregion
    }
}