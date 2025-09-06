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
    public class LocatarioControllerTests
    {
        private static LocatarioController controller = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var mockService = new Mock<ILocatarioService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new LocatarioProfile())).CreateMapper();

            mockService.Setup(service => service.GetAll(page, pageSize))
                .Returns(GetTestLocatarios());
            mockService.Setup(service => service.Get(1))
                .Returns(GetTargetLocatario());
            mockService.Setup(service => service.Edit(It.IsAny<Locatario>()))
                .Verifiable();
            mockService.Setup(service => service.Create(It.IsAny<Locatario>()))
                .Returns(4);
            mockService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();
            mockService.Setup(service => service.GetCount())
                .Returns(3);

            controller = new LocatarioController(mockService.Object, mapper);
            
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
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<LocatarioViewModel>));

            var listaLocatarios = (IEnumerable<LocatarioViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, listaLocatarios.Count());
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocatarioViewModel));
            LocatarioViewModel locatarioModel = (LocatarioViewModel)viewResult.ViewData.Model;
            Assert.IsTrue(1 == locatarioModel.Id);
            Assert.AreEqual("João Silva", locatarioModel.Nome);
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
            var result = controller.Create(GetNewLocatario());

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
            var result = controller.Create(GetNewLocatario());

            // Assert
            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocatarioViewModel));
        }

        [TestMethod()]
        public void EditTest_Get_Valido()
        {
            // Act
            var result = controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocatarioViewModel));
            LocatarioViewModel locatarioModel = (LocatarioViewModel)viewResult.ViewData.Model;
            Assert.IsTrue(1 == locatarioModel.Id);
            Assert.AreEqual("João Silva", locatarioModel.Nome);
        }

        [TestMethod()]
        public void EditTest_Post_Valido()
        {
            // Arrange
            // Forçar ModelState válido
            controller.ModelState.Clear();

            // Act
            var result = controller.Edit(1, GetTargetLocatarioModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void DeleteTest_Get_Valido()
        {
            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocatarioViewModel));
            LocatarioViewModel locatarioModel = (LocatarioViewModel)viewResult.ViewData.Model;
            Assert.IsTrue(1 == locatarioModel.Id);
            Assert.AreEqual("João Silva", locatarioModel.Nome);
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

        private LocatarioViewModel GetNewLocatario()
        {
            return new LocatarioViewModel
            {
                Id = 4,
                Nome = "Bruno Lima",
                Email = "bruno@gmail.com",
                Telefone1 = "51987654321",
                Cpf = "99988877766",
                Cep = "90000000",
                Logradouro = "Rua Porto Alegre",
                Numero = "300",
                Bairro = "Moinhos",
                Cidade = "Porto Alegre",
                Estado = "RS"
            };
        }

        private static Locatario GetTargetLocatario()
        {
            return new Locatario
            {
                Id = 1,
                Nome = "João Silva",
                Email = "joao@gmail.com",
                Telefone1 = "11999999999",
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua das Flores",
                Numero = "123",
                Bairro = "Vila Madalena",
                Cidade = "São Paulo",
                Estado = "SP"
            };
        }

        private LocatarioViewModel GetTargetLocatarioModel()
        {
            return new LocatarioViewModel
            {
                Id = 1,
                Nome = "João Silva",
                Email = "joao@gmail.com",
                Telefone1 = "11999999999",
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua das Flores",
                Numero = "123",
                Bairro = "Vila Madalena",
                Cidade = "São Paulo",
                Estado = "SP"
            };
        }

        private static IEnumerable<Locatario> GetTestLocatarios()
        {
            return
            [
                new Locatario { Id = 1, Nome = "João Silva", Email = "joao@gmail.com", Cpf = "12345678901" },
                new Locatario { Id = 2, Nome = "Maria Santos", Email = "maria@gmail.com", Cpf = "98765432100" },
                new Locatario { Id = 3, Nome = "Carlos Pereira", Email = "carlos@gmail.com", Cpf = "11122233344" }
            ];
        }
    }
}