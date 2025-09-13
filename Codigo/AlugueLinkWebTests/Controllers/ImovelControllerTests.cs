using AutoMapper;
using Core;
using Core.Service;
using AlugueLinkWEB.Mappers;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;

namespace AlugueLinkWEB.Controllers.Tests
{
    [TestClass()]
    public class ImovelControllerTests
    {
        private static ImovelController controller = null!;
        private Mock<HttpContext> mockHttpContext = null!;
        private Mock<HttpRequest> mockRequest = null!;
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
            mockLocadorService.Setup(service => service.GetAll(1, 1))
                .Returns(GetTestLocadores().Take(1));
            mockLocadorService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((int id) => GetTestLocadores().FirstOrDefault(l => l.Id == id));
            mockLocadorService.Setup(service => service.Create(It.IsAny<Locador>()))
                .Returns(3);

            mockAluguelService.Setup(service => service.AtualizarStatusAlugueis())
                .Verifiable();
            mockAluguelService.Setup(service => service.GetImoveisIndisponiveis())
                .Returns(new List<int>());
            mockAluguelService.Setup(service => service.IsImovelAvailable(It.IsAny<int>(), It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<int?>()))
                .Returns(true);

            controller = new ImovelController(mockImovelService.Object, mockLocadorService.Object, mockAluguelService.Object, mapper);
            
            var tempData = new Mock<ITempDataDictionary>();
            controller.TempData = tempData.Object;

            mockHttpContext = new Mock<HttpContext>();
            mockRequest = new Mock<HttpRequest>();
            var formCollection = new Mock<IFormCollection>();
            
            formCollection.Setup(f => f[It.IsAny<string>()])
                .Returns(new StringValues());
            
            mockRequest.Setup(r => r.Form)
                .Returns(formCollection.Object);
            
            mockHttpContext.Setup(c => c.Request)
                .Returns(mockRequest.Object);
            
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
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
            SetupValidFormData();
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
            SetupValidFormData();
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
        public void CreateTest_Post_ValorInvalido()
        {
            // Arrange
            var imovelInvalido = GetNewImovel();
            imovelInvalido.ValorStr = "invalid";
            SetupInvalidFormData("ValorStr", "invalid");
            controller.ModelState.Clear();

            // Act
            var result = controller.Create(imovelInvalido);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
        }

        [TestMethod()]
        public void CreateTest_Post_AreaInvalida()
        {
            // Arrange
            var imovelInvalido = GetNewImovel();
            imovelInvalido.AreaStr = "abc";
            SetupInvalidFormData("AreaStr", "abc");
            controller.ModelState.Clear();

            // Act
            var result = controller.Create(imovelInvalido);

            // Assert
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
            Assert.IsNotNull(imovelModel.AreaStr);
            Assert.IsNotNull(imovelModel.ValorStr);
        }

        [TestMethod()]
        public void EditTest_Post_Valido()
        {
            // Arrange
            SetupValidFormData();
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
        public void EditTest_Post_ValorInvalido()
        {
            // Arrange
            var imovelInvalido = GetTargetImovelModel();
            imovelInvalido.ValorStr = "3500.00";
            SetupInvalidFormData("ValorStr", "3500.00");
            controller.ModelState.Clear();

            // Act
            var result = controller.Edit(1, imovelInvalido);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
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

        private void SetupValidFormData()
        {
            var formCollection = new Mock<IFormCollection>();
            formCollection.Setup(f => f["ValorStr"])
                .Returns(new StringValues("4000,00"));
            formCollection.Setup(f => f["AreaStr"])
                .Returns(new StringValues("150,00"));
            
            mockRequest.Setup(r => r.Form)
                .Returns(formCollection.Object);
        }

        private void SetupInvalidFormData(string fieldName, string value)
        {
            var formCollection = new Mock<IFormCollection>();
            formCollection.Setup(f => f[fieldName])
                .Returns(new StringValues(value));
            formCollection.Setup(f => f[It.Is<string>(s => s != fieldName)])
                .Returns(new StringValues());
            
            mockRequest.Setup(r => r.Form)
                .Returns(formCollection.Object);
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
                AreaStr = "150,00",
                Area = 150.00m,
                VagasGaragem = 2,
                ValorStr = "4000,00",
                Valor = 4000.00m,
                Descricao = "Casa moderna",
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
                AreaStr = "120,5",
                Area = 120.50m,
                VagasGaragem = 1,
                ValorStr = "3500,00",
                Valor = 3500.00m,
                Descricao = "Apartamento amplo",
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