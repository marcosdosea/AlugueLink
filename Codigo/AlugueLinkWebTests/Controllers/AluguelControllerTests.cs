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
    }
}
