using AutoMapper;
using Core;
using Core.Service;
using AlugueLinkWEB.Mappers;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;

namespace AlugueLinkWEB.Controllers.Tests
{
    [TestClass()]
    public class AluguelControllerTests
    {
        private static AluguelController controller = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
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

            // Setup TempData para evitar NullReferenceException
            var tempData = new Mock<ITempDataDictionary>();
            controller.TempData = tempData.Object;
        }

        [TestMethod()]
        public void IndexTest_Valido()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<AluguelViewModel>));

            var listaAlugueis = (IEnumerable<AluguelViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, listaAlugueis.Count());
        }

        [TestMethod()]
        public void IndexTest_ComFiltro_Ativos()
        {
            // Act
            var result = controller.Index(page, pageSize, "ativos");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<AluguelViewModel>));
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AluguelViewModel));
            AluguelViewModel aluguelModel = (AluguelViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, aluguelModel.Id);
            Assert.AreEqual("A", aluguelModel.Status);
        }

        [TestMethod()]
        public void DetailsTest_NotFound()
        {
            // Arrange
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockImovelService = new Mock<IImovelService>();

            mockAluguelService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Aluguel?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new AluguelProfile())).CreateMapper();

            var testController = new AluguelController(mockAluguelService.Object, mockLocatarioService.Object, mockImovelService.Object, mapper);

            // Act
            var result = testController.Details(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod()]
        public void CreateTest_Get_Valido()
        {
            // Act
            var result = controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void CreateTest_Post_Valido()
        {
            // Arrange
            // For ar ModelState v lido
            controller.ModelState.Clear();

            // Act
            var result = controller.Create(GetNewAluguel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void CreateTest_Post_Invalido()
        {
            // Arrange
            controller.ModelState.AddModelError("DataInicio", "Campo requerido");

            // Act
            var result = controller.Create(GetNewAluguel());

            // Assert
            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AluguelViewModel));
        }

        [TestMethod()]
        public void EditTest_Get_Valido()
        {
            // Act
            var result = controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AluguelViewModel));
            AluguelViewModel aluguelModel = (AluguelViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, aluguelModel.Id);
            Assert.AreEqual("A", aluguelModel.Status);
        }

        [TestMethod()]
        public void EditTest_Get_NotFound()
        {
            // Arrange
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockImovelService = new Mock<IImovelService>();

            mockAluguelService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Aluguel?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new AluguelProfile())).CreateMapper();

            var testController = new AluguelController(mockAluguelService.Object, mockLocatarioService.Object, mockImovelService.Object, mapper);

            // Act
            var result = testController.Edit(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod()]
        public void EditTest_Post_Valido()
        {
            // Arrange
            // For ar ModelState v lido
            controller.ModelState.Clear();

            // Act
            var result = controller.Edit(1, GetTargetAluguelModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void EditTest_Post_IdMismatch()
        {
            // Arrange
            controller.ModelState.Clear();
            var model = GetTargetAluguelModel();
            model.Id = 2; // ID diferente do par metro

            // Act
            var result = controller.Edit(1, model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod()]
        public void DeleteTest_Get_Valido()
        {
            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AluguelViewModel));
            AluguelViewModel aluguelModel = (AluguelViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, aluguelModel.Id);
            Assert.AreEqual("A", aluguelModel.Status);
        }

        [TestMethod()]
        public void DeleteTest_Get_NotFound()
        {
            // Arrange
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockImovelService = new Mock<IImovelService>();

            mockAluguelService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Aluguel?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new AluguelProfile())).CreateMapper();

            var testController = new AluguelController(mockAluguelService.Object, mockLocatarioService.Object, mockImovelService.Object, mapper);

            // Act
            var result = testController.Delete(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod()]
        public void DeleteTest_Post_Valido()
        {
            // Act
            var result = controller.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        private AluguelViewModel GetNewAluguel()
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

        private AluguelViewModel GetTargetAluguelModel()
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

        private static IEnumerable<Aluguel> GetTestAlugueis()
        {
            return
            [
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
            ];
        }

        private static IEnumerable<Locatario> GetTestLocatarios()
        {
            return
            [
                new Locatario { Id = 1, Nome = "Jo o Silva", Email = "joao@gmail.com", Cpf = "12345678901" },
                new Locatario { Id = 2, Nome = "Maria Santos", Email = "maria@gmail.com", Cpf = "98765432100" }
            ];
        }

        private static IEnumerable<Imovel> GetTestImoveis()
        {
            return
            [
                new Imovel { Id = 1, Logradouro = "Rua das Flores", Cidade = "S o Paulo", Tipo = "A", Valor = 3500.00m },
                new Imovel { Id = 2, Logradouro = "Av. Copacabana", Cidade = "Rio de Janeiro", Tipo = "C", Valor = 5000.00m }
            ];
        }
    }
}
