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
    public class LocadorControllerTests
    {
        private static LocadorController controller = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
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
            
            var tempData = new Mock<ITempDataDictionary>();
            controller.TempData = tempData.Object;
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            var result = controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<LocadorViewModel>));

            var lista = (IEnumerable<LocadorViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, lista.Count());
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            var result = controller.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocadorViewModel));
            
            var model = (LocadorViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("João Proprietário", model.Nome);
        }

        [TestMethod]
        public void DetailsTest_NotFound()
        {
            var mockService = new Mock<ILocadorService>();
            mockService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Locador?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new LocadorProfile())).CreateMapper();

            var testController = new LocadorController(mockService.Object, mapper);

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

            var result = controller.Create(GetNewLocadorModel());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.IsNull(redirect.ControllerName);
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public void CreateTest_Post_Invalido()
        {
            controller.ModelState.AddModelError("Nome", "Campo requerido");

            var result = controller.Create(GetNewLocadorModel());

            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocadorViewModel));
        }

        [TestMethod]
        public void EditTest_Get_Valido()
        {
            var result = controller.Edit(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocadorViewModel));
            
            var model = (LocadorViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("João Proprietário", model.Nome);
        }

        [TestMethod]
        public void EditTest_Get_NotFound()
        {
            var mockService = new Mock<ILocadorService>();
            mockService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Locador?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new LocadorProfile())).CreateMapper();

            var testController = new LocadorController(mockService.Object, mapper);

            var result = testController.Edit(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void EditTest_Post_Valido()
        {
            controller.ModelState.Clear();

            var result = controller.Edit(1, GetTargetLocadorModel());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.IsNull(redirect.ControllerName);
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public void EditTest_Post_IdMismatch()
        {
            controller.ModelState.Clear();
            var model = GetTargetLocadorModel();
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
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(LocadorViewModel));
            
            var model = (LocadorViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("João Proprietário", model.Nome);
        }

        [TestMethod]
        public void DeleteTest_Get_NotFound()
        {
            var mockService = new Mock<ILocadorService>();
            mockService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Locador?)null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new LocadorProfile())).CreateMapper();

            var testController = new LocadorController(mockService.Object, mapper);

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

        private static LocadorViewModel GetNewLocadorModel()
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

        private static List<Locador> GetTestLocadores()
        {
            return new List<Locador>
            {
                new Locador { Id = 1, Nome = "João Proprietário", Email = "joao@gmail.com", Cpf = "12345678901" },
                new Locador { Id = 2, Nome = "Maria Proprietária", Email = "maria@gmail.com", Cpf = "98765432100" },
                new Locador { Id = 3, Nome = "Carlos Silva", Email = "carlos@gmail.com", Cpf = "11122233344" }
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

        private static LocadorViewModel GetTargetLocadorModel()
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
    }
}