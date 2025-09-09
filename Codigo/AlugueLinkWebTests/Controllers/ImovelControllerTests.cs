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
    public class ImovelControllerTests
    {
        private static ImovelController controller = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var mockImovelService = new Mock<IImovelService>();
            var mockLocadorService = new Mock<ILocadorService>();
            var mockAluguelService = new Mock<IAluguelService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new ImovelProfile())).CreateMapper();

            mockImovelService.Setup(service => service.GetAll(page, pageSize))
                .Returns(GetTestImoveis());
            mockImovelService.Setup(service => service.Get(1))
                .Returns(GetTargetImovel());
            mockImovelService.Setup(service => service.Edit(It.IsAny<Imovel>()))
                .Verifiable();
            mockImovelService.Setup(service => service.Create(It.IsAny<Imovel>()))
                .Returns(4);
            mockImovelService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();
            mockImovelService.Setup(service => service.GetCount())
                .Returns(3);
            
            mockLocadorService.Setup(service => service.GetAll(1, 1000))
                .Returns(GetTestLocadores());

            controller = new ImovelController(mockImovelService.Object, mockLocadorService.Object, mockAluguelService.Object, mapper);
            
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
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<ImovelViewModel>));

            var listaImoveis = (IEnumerable<ImovelViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, listaImoveis.Count());
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
            ImovelViewModel imovelModel = (ImovelViewModel)viewResult.ViewData.Model;
            Assert.IsTrue(1 == imovelModel.Id);
            Assert.AreEqual("Rua das Flores", imovelModel.Logradouro);
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
            var result = controller.Create(GetNewImovel());

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
            controller.ModelState.AddModelError("Logradouro", "Campo requerido");

            // Act
            var result = controller.Create(GetNewImovel());

            // Assert
            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
        }

        [TestMethod()]
        public void EditTest_Get_Valido()
        {
            // Act
            var result = controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
            ImovelViewModel imovelModel = (ImovelViewModel)viewResult.ViewData.Model;
            Assert.IsTrue(1 == imovelModel.Id);
            Assert.AreEqual("Rua das Flores", imovelModel.Logradouro);
        }

        [TestMethod()]
        public void EditTest_Post_Valido()
        {
            // Arrange
            // Forçar ModelState válido
            controller.ModelState.Clear();

            // Act
            var result = controller.Edit(1, GetTargetImovelModel());

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
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
            ImovelViewModel imovelModel = (ImovelViewModel)viewResult.ViewData.Model;
            Assert.IsTrue(1 == imovelModel.Id);
            Assert.AreEqual("Rua das Flores", imovelModel.Logradouro);
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

        private ImovelViewModel GetNewImovel()
        {
            return new ImovelViewModel
            {
                Id = 4,
                Cep = "90000000",
                Logradouro = "Rua Porto Alegre",
                Numero = "300",
                Bairro = "Moinhos",
                Cidade = "Porto Alegre",
                Estado = "RS",
                Tipo = "casa",
                Quartos = 3,
                Banheiros = 2,
                Area = 150.00m,
                Valor = 4000.00m,
                LocadorId = 1
            };
        }

        private static Imovel GetTargetImovel()
        {
            return new Imovel
            {
                Id = 1,
                Cep = "01234567",
                Logradouro = "Rua das Flores",
                Numero = "123",
                Bairro = "Vila Madalena",
                Cidade = "São Paulo",
                Estado = "SP",
                Tipo = "A",
                Quartos = 3,
                Banheiros = 2,
                Area = 120.50m,
                Valor = 3500.00m,
                IdLocador = 1
            };
        }

        private ImovelViewModel GetTargetImovelModel()
        {
            return new ImovelViewModel
            {
                Id = 1,
                Cep = "01234567",
                Logradouro = "Rua das Flores",
                Numero = "123",
                Bairro = "Vila Madalena",
                Cidade = "São Paulo",
                Estado = "SP",
                Tipo = "apartamento",
                Quartos = 3,
                Banheiros = 2,
                Area = 120.50m,
                Valor = 3500.00m,
                LocadorId = 1
            };
        }

        private static IEnumerable<Imovel> GetTestImoveis()
        {
            return
            [
                new Imovel { Id = 1, Logradouro = "Rua das Flores", Cidade = "São Paulo", Tipo = "A", Valor = 3500.00m },
                new Imovel { Id = 2, Logradouro = "Av. Copacabana", Cidade = "Rio de Janeiro", Tipo = "C", Valor = 5000.00m },
                new Imovel { Id = 3, Logradouro = "Rua da Bahia", Cidade = "Belo Horizonte", Tipo = "PC", Valor = 2500.00m }
            ];
        }

        private static IEnumerable<Locador> GetTestLocadores()
        {
            return
            [
                new Locador { Id = 1, Nome = "Pedro Proprietário", Email = "pedro@gmail.com" },
                new Locador { Id = 2, Nome = "Ana Proprietária", Email = "ana@gmail.com" }
            ];
        }
    }
}