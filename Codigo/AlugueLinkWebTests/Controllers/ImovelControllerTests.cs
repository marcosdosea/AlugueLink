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
    [TestClass]
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

        [TestMethod]
        public void IndexTest_Valido()
        {
            var result = controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<ImovelViewModel>));

            var lista = (IEnumerable<ImovelViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, lista.Count());
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            var result = controller.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
            
            var model = (ImovelViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("Rua das Flores", model.Logradouro);
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
            SetupValidFormData();
            controller.ModelState.Clear();

            var result = controller.Create(GetNewImovelModel());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.IsNull(redirect.ControllerName);
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public void CreateTest_Post_Invalido()
        {
            SetupValidFormData();
            controller.ModelState.AddModelError("Logradouro", "Campo requerido");

            var result = controller.Create(GetNewImovelModel());

            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
        }

        [TestMethod]
        public void CreateTest_Post_ValorInvalido()
        {
            var imovelInvalido = GetNewImovelModel();
            imovelInvalido.ValorStr = "invalid";
            SetupInvalidFormData("ValorStr", "invalid");
            controller.ModelState.Clear();

            var result = controller.Create(imovelInvalido);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
        }

        [TestMethod]
        public void CreateTest_Post_AreaInvalida()
        {
            var imovelInvalido = GetNewImovelModel();
            imovelInvalido.AreaStr = "abc";
            SetupInvalidFormData("AreaStr", "abc");
            controller.ModelState.Clear();

            var result = controller.Create(imovelInvalido);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
        }

        [TestMethod]
        public void EditTest_Get_Valido()
        {
            var result = controller.Edit(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
            
            var model = (ImovelViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("Rua das Flores", model.Logradouro);
            Assert.IsNotNull(model.AreaStr);
            Assert.IsNotNull(model.ValorStr);
        }

        [TestMethod]
        public void EditTest_Post_Valido()
        {
            SetupValidFormData();
            controller.ModelState.Clear();

            var result = controller.Edit(1, GetTargetImovelModel());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.IsNull(redirect.ControllerName);
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public void EditTest_Post_ValorInvalido()
        {
            var imovelInvalido = GetTargetImovelModel();
            imovelInvalido.ValorStr = "3500.00";
            SetupInvalidFormData("ValorStr", "3500.00");
            controller.ModelState.Clear();

            var result = controller.Edit(1, imovelInvalido);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
        }

        [TestMethod]
        public void DeleteTest_Get_Valido()
        {
            var result = controller.Delete(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ImovelViewModel));
            
            var model = (ImovelViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("Rua das Flores", model.Logradouro);
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

        private static ImovelViewModel GetNewImovelModel()
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

        private static List<Imovel> GetTestImoveis()
        {
            return new List<Imovel>
            {
                new Imovel { Id = 1, Logradouro = "Rua das Flores", Cidade = "São Paulo", Tipo = "A", Valor = 3500.00m },
                new Imovel { Id = 2, Logradouro = "Av. Copacabana", Cidade = "Rio de Janeiro", Tipo = "C", Valor = 5000.00m },
                new Imovel { Id = 3, Logradouro = "Rua da Bahia", Cidade = "Belo Horizonte", Tipo = "PC", Valor = 2500.00m }
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

        private static ImovelViewModel GetTargetImovelModel()
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

        private static List<Locador> GetTestLocadores()
        {
            return new List<Locador>
            {
                new Locador { Id = 1, Nome = "Pedro Proprietário", Email = "pedro@gmail.com" },
                new Locador { Id = 2, Nome = "Ana Proprietária", Email = "ana@gmail.com" }
            };
        }
    }
}