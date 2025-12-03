using AutoMapper;
using Core;
using Core.Service;
using AlugueLinkWEB.Mappers;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace AlugueLinkWEB.Controllers.Tests
{
    [TestClass]
    public class AluguelControllerTests
    {
        private static AluguelController controller = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockImovelService = new Mock<IImovelService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new AluguelProfile())).CreateMapper();

            mockAluguelService.Setup(service => service.GetAll(page, pageSize))
                .Returns(GetTestAlugueis());
            mockAluguelService.Setup(service => service.Get(1))
                .Returns(GetTargetAluguel());
            mockAluguelService.Setup(service => service.Edit(It.IsAny<Aluguel>()))
                .Verifiable();
            mockAluguelService.Setup(service => service.Create(It.IsAny<Aluguel>()))
                .Returns(4);
            mockAluguelService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();
            mockAluguelService.Setup(service => service.GetCount())
                .Returns(3);
            mockAluguelService.Setup(service => service.AtualizarStatusAlugueis())
                .Verifiable();
            mockAluguelService.Setup(service => service.IsImovelAvailable(It.IsAny<int>(), It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<int?>()))
                .Returns(true);
            mockAluguelService.Setup(service => service.IsLocatarioAvailable(It.IsAny<int>(), It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<int?>()))
                .Returns(true);
            mockAluguelService.Setup(service => service.GetImoveisIndisponiveis())
                .Returns(new List<int>());

            mockLocatarioService.Setup(service => service.GetAll(1, 1000))
                .Returns(GetTestLocatarios());
            mockLocatarioService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((int id) => GetTestLocatarios().FirstOrDefault(l => l.Id == id));

            mockImovelService.Setup(service => service.GetAll(1, 1000))
                .Returns(GetTestImoveis());
            mockImovelService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((int id) => GetTestImoveis().FirstOrDefault(i => i.Id == id));

            controller = new AluguelController(mockAluguelService.Object, mockLocatarioService.Object, mockImovelService.Object, mapper);

            var tempData = new Mock<ITempDataDictionary>();
            controller.TempData = tempData.Object;
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            var result = controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<AluguelViewModel>));

            var lista = (IEnumerable<AluguelViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, lista.Count());
        }

        [TestMethod]
        public void IndexTest_ComFiltro_Ativos()
        {
            var result = controller.Index(page, pageSize, "ativos");

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<AluguelViewModel>));
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            var result = controller.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AluguelViewModel));
            
            var model = (AluguelViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("A", model.Status);
        }

        [TestMethod]
        public void DetailsTest_NotFound()
        {
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockImovelService = new Mock<IImovelService>();

            mockAluguelService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Aluguel?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new AluguelProfile())).CreateMapper();

            var testController = new AluguelController(mockAluguelService.Object, mockLocatarioService.Object, mockImovelService.Object, mapper);

            var result = testController.Details(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void CreateTest_Get_Valido()
        {
            var result = controller.Create();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CreateTest_Post_Valido()
        {
            controller.ModelState.Clear();

            var result = controller.Create(GetNewAluguelModel());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.IsNull(redirect.ControllerName);
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public void CreateTest_Post_Invalido()
        {
            controller.ModelState.AddModelError("DataInicio", "Campo requerido");

            var result = controller.Create(GetNewAluguelModel());

            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AluguelViewModel));
        }

        [TestMethod]
        public void EditTest_Get_Valido()
        {
            var result = controller.Edit(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AluguelViewModel));
            
            var model = (AluguelViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("A", model.Status);
        }

        [TestMethod]
        public void EditTest_Get_NotFound()
        {
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockImovelService = new Mock<IImovelService>();

            mockAluguelService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Aluguel?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new AluguelProfile())).CreateMapper();

            var testController = new AluguelController(mockAluguelService.Object, mockLocatarioService.Object, mockImovelService.Object, mapper);

            var result = testController.Edit(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void EditTest_Post_Valido()
        {
            controller.ModelState.Clear();

            var result = controller.Edit(1, GetTargetAluguelModel());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.IsNull(redirect.ControllerName);
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public void EditTest_Post_IdMismatch()
        {
            controller.ModelState.Clear();
            var model = GetTargetAluguelModel();
            model.Id = 2;

            var result = controller.Edit(1, model);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void DeleteTest_Get_Valido()
        {
            var result = controller.Delete(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AluguelViewModel));
            
            var model = (AluguelViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("A", model.Status);
        }

        [TestMethod]
        public void DeleteTest_Get_NotFound()
        {
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockImovelService = new Mock<IImovelService>();

            mockAluguelService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Aluguel?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new AluguelProfile())).CreateMapper();

            var testController = new AluguelController(mockAluguelService.Object, mockLocatarioService.Object, mockImovelService.Object, mapper);

            var result = testController.Delete(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void DeleteTest_Post_Valido()
        {
            var result = controller.DeleteConfirmed(1);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.IsNull(redirect.ControllerName);
            Assert.AreEqual("Index", redirect.ActionName);
        }

        private static AluguelViewModel GetNewAluguelModel()
        {
            return new AluguelViewModel
            {
                Id = 4,
                IdLocatario = 1,
                IdImovel = 1,
                DataInicio = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                DataFim = DateOnly.FromDateTime(DateTime.Now.AddDays(395)),
                DataAssinatura = DateOnly.FromDateTime(DateTime.Now),
                Status = "P"
            };
        }

        private static List<Aluguel> GetTestAlugueis()
        {
            return new List<Aluguel>
            {
                new Aluguel {
                    Id = 1, Idlocatario = 1, Idimovel = 1, Status = "A",
                    DataInicio = DateOnly.FromDateTime(DateTime.Now.AddMonths(-2)),
                    DataFim = DateOnly.FromDateTime(DateTime.Now.AddMonths(10)),
                    DataAssinatura = DateOnly.FromDateTime(DateTime.Now.AddMonths(-2))
                },
                new Aluguel {
                    Id = 2, Idlocatario = 2, Idimovel = 2, Status = "F",
                    DataInicio = DateOnly.FromDateTime(DateTime.Now.AddMonths(-12)),
                    DataFim = DateOnly.FromDateTime(DateTime.Now.AddMonths(-2)),
                    DataAssinatura = DateOnly.FromDateTime(DateTime.Now.AddMonths(-12))
                },
                new Aluguel {
                    Id = 3, Idlocatario = 1, Idimovel = 2, Status = "P",
                    DataInicio = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),
                    DataFim = DateOnly.FromDateTime(DateTime.Now.AddMonths(13)),
                    DataAssinatura = DateOnly.FromDateTime(DateTime.Now)
                }
            };
        }

        private static Aluguel GetTargetAluguel()
        {
            return new Aluguel
            {
                Id = 1,
                Idlocatario = 1,
                Idimovel = 1,
                DataInicio = DateOnly.FromDateTime(DateTime.Now.AddMonths(-2)),
                DataFim = DateOnly.FromDateTime(DateTime.Now.AddMonths(10)),
                DataAssinatura = DateOnly.FromDateTime(DateTime.Now.AddMonths(-2)),
                Status = "A"
            };
        }

        private static AluguelViewModel GetTargetAluguelModel()
        {
            return new AluguelViewModel
            {
                Id = 1,
                IdLocatario = 1,
                IdImovel = 1,
                DataInicio = DateOnly.FromDateTime(DateTime.Now.AddMonths(-2)),
                DataFim = DateOnly.FromDateTime(DateTime.Now.AddMonths(10)),
                DataAssinatura = DateOnly.FromDateTime(DateTime.Now.AddMonths(-2)),
                Status = "A"
            };
        }

        private static List<Locatario> GetTestLocatarios()
        {
            return new List<Locatario>
            {
                new Locatario { Id = 1, Nome = "João Silva", Email = "joao@gmail.com", Cpf = "12345678901" },
                new Locatario { Id = 2, Nome = "Maria Santos", Email = "maria@gmail.com", Cpf = "98765432100" }
            };
        }

        private static List<Imovel> GetTestImoveis()
        {
            return new List<Imovel>
            {
                new Imovel { Id = 1, Logradouro = "Rua das Flores", Cidade = "São Paulo", Tipo = "A", Valor = 3500.00m },
                new Imovel { Id = 2, Logradouro = "Av. Copacabana", Cidade = "Rio de Janeiro", Tipo = "C", Valor = 5000.00m }
            };
        }

        #region Testes de Fluxo de Controle e Validação

        /// <summary>Teste de Caminho Básico: cobertura dos caminhos do switch no Index.</summary>
        [TestMethod]
        [DataRow("ativos", "A", DisplayName = "Index_FiltroAtivos")]
        [DataRow("finalizados", "F", DisplayName = "Index_FiltroFinalizados")]
        [DataRow("pendentes", "P", DisplayName = "Index_FiltroPendentes")]
        public void Index_ComFiltro_RetornaApenasStatusEsperado(string filtro, string statusEsperado)
        {
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockImovelService = new Mock<IImovelService>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AluguelProfile())).CreateMapper();

            mockAluguelService.Setup(s => s.GetAll(It.IsAny<int>(), It.IsAny<int>())).Returns(GetTestAlugueis());
            mockAluguelService.Setup(s => s.GetCount()).Returns(3);
            mockAluguelService.Setup(s => s.AtualizarStatusAlugueis());

            var testController = new AluguelController(mockAluguelService.Object, mockLocatarioService.Object, mockImovelService.Object, mapper);

            var result = testController.Index(1, 10, filtro);

            var viewResult = (ViewResult)result;
            var lista = ((IEnumerable<AluguelViewModel>)viewResult.ViewData.Model!).ToList();
            Assert.IsTrue(lista.All(a => a.Status == statusEsperado));
            Assert.AreEqual(1, lista.Count);
        }

        /// <summary>Teste de Validação: DataFim anterior a DataInicio deve gerar erro.</summary>
        [TestMethod]
        public void Create_DataFimAnteriorDataInicio_RetornaErroValidacao()
        {
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockImovelService = new Mock<IImovelService>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AluguelProfile())).CreateMapper();

            mockLocatarioService.Setup(s => s.Get(It.IsAny<int>()))
                .Returns(new Locatario { Id = 1, Nome = "Test", Email = "t@t.com", Cpf = "12345678901" });
            mockImovelService.Setup(s => s.Get(It.IsAny<int>()))
                .Returns(new Imovel { Id = 1, Logradouro = "Test", Cidade = "Test", Tipo = "A", Valor = 1000m });
            mockAluguelService.Setup(s => s.IsImovelAvailable(It.IsAny<int>(), It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<int?>())).Returns(true);
            mockAluguelService.Setup(s => s.GetImoveisIndisponiveis()).Returns(new List<int>());
            mockLocatarioService.Setup(s => s.GetAll(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Locatario>());
            mockImovelService.Setup(s => s.GetAll(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Imovel>());

            var testController = new AluguelController(mockAluguelService.Object, mockLocatarioService.Object, mockImovelService.Object, mapper);
            testController.TempData = new Mock<ITempDataDictionary>().Object;

            var invalidModel = new AluguelViewModel
            {
                IdLocatario = 1, IdImovel = 1,
                DataInicio = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                DataFim = DateOnly.FromDateTime(DateTime.Now.AddDays(10)), // ANTES de DataInicio
                DataAssinatura = DateOnly.FromDateTime(DateTime.Now),
                Status = "P"
            };

            var result = testController.Create(invalidModel);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsTrue(testController.ModelState.ContainsKey("DataFim"));
            mockAluguelService.Verify(s => s.Create(It.IsAny<Aluguel>()), Times.Never);
        }

        /// <summary>Teste de Exceção: Delete deve tratar exceção e redirecionar com mensagem de erro.</summary>
        [TestMethod]
        public void DeleteConfirmed_QuandoServiceFalha_RedirecionaComMensagemErro()
        {
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockImovelService = new Mock<IImovelService>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AluguelProfile())).CreateMapper();

            mockAluguelService.Setup(s => s.Delete(It.IsAny<int>()))
                .Throws(new InvalidOperationException("Erro simulado"));

            var testController = new AluguelController(mockAluguelService.Object, mockLocatarioService.Object, mockImovelService.Object, mapper);

            var tempDataDict = new Dictionary<string, object?>();
            var mockTempData = new Mock<ITempDataDictionary>();
            mockTempData.Setup(t => t[It.IsAny<string>()])
                .Returns((string key) => tempDataDict.ContainsKey(key) ? tempDataDict[key] : null);
            mockTempData.SetupSet(t => t[It.IsAny<string>()] = It.IsAny<object>())
                .Callback<string, object?>((key, value) => tempDataDict[key] = value);
            testController.TempData = mockTempData.Object;

            var result = testController.DeleteConfirmed(1);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
            Assert.IsTrue(tempDataDict.ContainsKey("ErrorMessage"));
        }

        #endregion
    }
}
