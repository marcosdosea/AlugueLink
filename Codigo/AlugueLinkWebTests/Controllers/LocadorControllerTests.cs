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
    [TestClass()]
    public class LocadorControllerTests
    {
        private static LocadorController controller = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var mockService = new Mock<ILocadorService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new LocadorProfile())).CreateMapper();

            mockService.Setup(service => service.GetAll(page, pageSize))
                .Returns(GetTestLocadores());
            mockService.Setup(service => service.Get(1))
                .Returns(GetTargetLocador());
            mockService.Setup(service => service.Edit(It.IsAny<Locador>()))
                .Verifiable();
            mockService.Setup(service => service.Create(It.IsAny<Locador>()))
                .Returns(4);
            mockService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();
            mockService.Setup(service => service.GetCount())
                .Returns(3);

            controller = new LocadorController(mockService.Object, mapper);
            
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
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<LocadorViewModel>));

            var listaLocadores = (IEnumerable<LocadorViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, listaLocadores.Count());
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocadorViewModel));
            LocadorViewModel locadorModel = (LocadorViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, locadorModel.Id);
            Assert.AreEqual("João Proprietário", locadorModel.Nome);
        }

        [TestMethod()]
        public void DetailsTest_NotFound()
        {
            // Arrange
            var mockService = new Mock<ILocadorService>();
            mockService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Locador?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new LocadorProfile())).CreateMapper();

            var testController = new LocadorController(mockService.Object, mapper);

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
            // Forçar ModelState válido
            controller.ModelState.Clear();

            // Act
            var result = controller.Create(GetNewLocador());

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
            controller.ModelState.AddModelError("Nome", "Campo requerido");

            // Act
            var result = controller.Create(GetNewLocador());

            // Assert
            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocadorViewModel));
        }

        [TestMethod()]
        public void EditTest_Get_Valido()
        {
            // Act
            var result = controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocadorViewModel));
            LocadorViewModel locadorModel = (LocadorViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, locadorModel.Id);
            Assert.AreEqual("João Proprietário", locadorModel.Nome);
        }

        [TestMethod()]
        public void EditTest_Get_NotFound()
        {
            // Arrange
            var mockService = new Mock<ILocadorService>();
            mockService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Locador?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new LocadorProfile())).CreateMapper();

            var testController = new LocadorController(mockService.Object, mapper);

            // Act
            var result = testController.Edit(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod()]
        public void EditTest_Post_Valido()
        {
            // Arrange
            // Forçar ModelState válido
            controller.ModelState.Clear();

            // Act
            var result = controller.Edit(1, GetTargetLocadorModel());

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
            var model = GetTargetLocadorModel();
            model.Id = 2; // ID diferente do parâmetro

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
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocadorViewModel));
            LocadorViewModel locadorModel = (LocadorViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, locadorModel.Id);
            Assert.AreEqual("João Proprietário", locadorModel.Nome);
        }

        [TestMethod()]
        public void DeleteTest_Get_NotFound()
        {
            // Arrange
            var mockService = new Mock<ILocadorService>();
            mockService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Locador?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new LocadorProfile())).CreateMapper();

            var testController = new LocadorController(mockService.Object, mapper);

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

        private LocadorViewModel GetNewLocador()
        {
            return new LocadorViewModel
            {
                Id = 4,
                Nome = "Ana Proprietária",
                Email = "ana@gmail.com",
                Telefone = "31999999999",
                Cpf = "55566677788"
            };
        }

        private static Locador GetTargetLocador()
        {
            return new Locador
            {
                Id = 1,
                Nome = "João Proprietário",
                Email = "joao@gmail.com",
                Telefone = "11999999999",
                Cpf = "12345678901"
            };
        }

        private LocadorViewModel GetTargetLocadorModel()
        {
            return new LocadorViewModel
            {
                Id = 1,
                Nome = "João Proprietário",
                Email = "joao@gmail.com",
                Telefone = "11999999999",
                Cpf = "12345678901"
            };
        }

        private static IEnumerable<Locador> GetTestLocadores()
        {
            return
            [
                new Locador { Id = 1, Nome = "João Proprietário", Email = "joao@gmail.com", Cpf = "12345678901" },
                new Locador { Id = 2, Nome = "Maria Proprietária", Email = "maria@gmail.com", Cpf = "98765432100" },
                new Locador { Id = 3, Nome = "Carlos Silva", Email = "carlos@gmail.com", Cpf = "11122233344" }
            ];
        }
    }
}