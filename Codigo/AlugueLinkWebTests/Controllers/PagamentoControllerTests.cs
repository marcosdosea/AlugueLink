using AutoMapper;
using Core;
using Core.Service;
using AlugueLinkWEB.Mappers;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace AlugueLinkWEB.Controllers.Tests
{
    [TestClass]
    public class PagamentoControllerTests
    {
        private static PagamentoController controller = null!;
        private readonly int page = 1;
        private readonly int pageSize = 10;

        [TestInitialize]
        public void Initialize()
        {
            var mockPagamentoService = new Mock<IPagamentoService>();
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocadorService = new Mock<ILocadorService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new PagamentoProfile())).CreateMapper();

            mockPagamentoService.Setup(service => service.GetByLocador(1, page, pageSize))
                .Returns(GetTestPagamentos());
            mockPagamentoService.Setup(service => service.Get(1))
                .Returns(GetTargetPagamento());
            mockPagamentoService.Setup(service => service.Edit(It.IsAny<Pagamento>()))
                .Verifiable();
            mockPagamentoService.Setup(service => service.Create(It.IsAny<Pagamento>()))
                .Returns(4);
            mockPagamentoService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();
            mockPagamentoService.Setup(service => service.GetCountByLocador(1))
                .Returns(3);

            mockAluguelService.Setup(service => service.GetByLocador(1))
                .Returns(GetTestAlugueis());
            mockAluguelService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((int id) => GetTestAlugueis().FirstOrDefault(a => a.Id == id));

            mockLocadorService.Setup(service => service.GetByEmail("test@test.com"))
                .Returns(GetTestLocador());
            mockLocadorService.Setup(service => service.Get(1))
                .Returns(GetTestLocador());

            controller = new PagamentoController(mockPagamentoService.Object, mockAluguelService.Object, mockLocadorService.Object, mapper);

            var tempData = new Mock<ITempDataDictionary>();
            controller.TempData = tempData.Object;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test@test.com"),
                new Claim("NomeCompleto", "Usuário Teste")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User).Returns(principal);
            
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
            controller.ControllerContext = controllerContext;
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            var result = controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<PagamentoViewModel>));

            var lista = (IEnumerable<PagamentoViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, lista.Count());
        }

        [TestMethod]
        public void IndexTest_ComPaginacao()
        {
            var result = controller.Index(page, pageSize);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<PagamentoViewModel>));
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            var result = controller.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PagamentoViewModel));
            
            var model = (PagamentoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual(2000.00m, model.Valor);
        }

        [TestMethod]
        public void DetailsTest_NotFound()
        {
            var mockPagamentoService = new Mock<IPagamentoService>();
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocadorService = new Mock<ILocadorService>();

            mockPagamentoService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Pagamento?)null);

            mockLocadorService.Setup(service => service.GetByEmail("test@test.com"))
                .Returns(GetTestLocador());

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new PagamentoProfile())).CreateMapper();

            var testController = new PagamentoController(mockPagamentoService.Object, mockAluguelService.Object, mockLocadorService.Object, mapper);

            var tempData = new Mock<ITempDataDictionary>();
            testController.TempData = tempData.Object;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test@test.com")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User).Returns(principal);
            
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
            testController.ControllerContext = controllerContext;

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

            var result = controller.Create(GetNewPagamentoModel());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.IsNull(redirect.ControllerName);
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public void CreateTest_Post_Invalido()
        {
            controller.ModelState.AddModelError("TipoPagamento", "Campo requerido");

            var result = controller.Create(GetNewPagamentoModel());

            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PagamentoViewModel));
        }

        [TestMethod]
        public void EditTest_Get_Valido()
        {
            var result = controller.Edit(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PagamentoViewModel));
            
            var model = (PagamentoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual(2000.00m, model.Valor);
        }

        [TestMethod]
        public void EditTest_Get_NotFound()
        {
            var mockPagamentoService = new Mock<IPagamentoService>();
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocadorService = new Mock<ILocadorService>();

            mockPagamentoService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Pagamento?)null);

            mockLocadorService.Setup(service => service.GetByEmail("test@test.com"))
                .Returns(GetTestLocador());

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new PagamentoProfile())).CreateMapper();

            var testController = new PagamentoController(mockPagamentoService.Object, mockAluguelService.Object, mockLocadorService.Object, mapper);

            var tempData = new Mock<ITempDataDictionary>();
            testController.TempData = tempData.Object;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test@test.com")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User).Returns(principal);
            
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
            testController.ControllerContext = controllerContext;

            var result = testController.Edit(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void EditTest_Post_Valido()
        {
            controller.ModelState.Clear();

            var result = controller.Edit(1, GetTargetPagamentoModel());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.IsNull(redirect.ControllerName);
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public void EditTest_Post_IdMismatch()
        {
            controller.ModelState.Clear();
            var model = GetTargetPagamentoModel();
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
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PagamentoViewModel));
            
            var model = (PagamentoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual(2000.00m, model.Valor);
        }

        [TestMethod]
        public void DeleteTest_Get_NotFound()
        {
            var mockPagamentoService = new Mock<IPagamentoService>();
            var mockAluguelService = new Mock<IAluguelService>();
            var mockLocadorService = new Mock<ILocadorService>();

            mockPagamentoService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns((Pagamento?)null);

            mockLocadorService.Setup(service => service.GetByEmail("test@test.com"))
                .Returns(GetTestLocador());

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new PagamentoProfile())).CreateMapper();

            var testController = new PagamentoController(mockPagamentoService.Object, mockAluguelService.Object, mockLocadorService.Object, mapper);

            var tempData = new Mock<ITempDataDictionary>();
            testController.TempData = tempData.Object;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test@test.com")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User).Returns(principal);
            
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
            testController.ControllerContext = controllerContext;

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

        [TestMethod]
        public void GetValorAluguelTest_Valido()
        {
            var result = controller.GetValorAluguel(1);

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var jsonResult = (JsonResult)result;
            Assert.IsNotNull(jsonResult.Value);
        }

        private static PagamentoViewModel GetNewPagamentoModel()
        {
            return new PagamentoViewModel
            {
                Id = 4,
                Valor = 2000.00m,
                TipoPagamento = "P",
                AluguelId = 1,
                DataPagamento = DateOnly.FromDateTime(DateTime.Now),
                HoraPagamento = TimeOnly.FromDateTime(DateTime.Now)
            };
        }

        private static List<Pagamento> GetTestPagamentos()
        {
            return new List<Pagamento>
            {
                new Pagamento {
                    Id = 1, Valor = 2000.00m, DataPagamento = DateTime.Now.AddDays(-30),
                    TipoPagamento = "P", Idaluguel = 1,
                    IdaluguelNavigation = new Aluguel
                    {
                        Id = 1, Idlocatario = 1, Idimovel = 1, Status = "A",
                        IdimovelNavigation = new Imovel { Id = 1, IdLocador = 1, Valor = 2000.00m, Logradouro = "Rua A", Numero = "100", Bairro = "Centro", Cidade = "São Paulo" },
                        IdlocatarioNavigation = new Locatario { Id = 1, Nome = "Maria Silva" }
                    }
                },
                new Pagamento {
                    Id = 2, Valor = 3000.00m, DataPagamento = DateTime.Now.AddDays(-15),
                    TipoPagamento = "CC", Idaluguel = 2,
                    IdaluguelNavigation = new Aluguel
                    {
                        Id = 2, Idlocatario = 2, Idimovel = 2, Status = "A",
                        IdimovelNavigation = new Imovel { Id = 2, IdLocador = 1, Valor = 3000.00m, Logradouro = "Rua B", Numero = "200", Bairro = "Vila Nova", Cidade = "São Paulo" },
                        IdlocatarioNavigation = new Locatario { Id = 2, Nome = "Carlos Santos" }
                    }
                },
                new Pagamento {
                    Id = 3, Valor = 2000.00m, DataPagamento = DateTime.Now.AddDays(-5),
                    TipoPagamento = "B", Idaluguel = 1,
                    IdaluguelNavigation = new Aluguel
                    {
                        Id = 1, Idlocatario = 1, Idimovel = 1, Status = "A",
                        IdimovelNavigation = new Imovel { Id = 1, IdLocador = 1, Valor = 2000.00m, Logradouro = "Rua A", Numero = "100", Bairro = "Centro", Cidade = "São Paulo" },
                        IdlocatarioNavigation = new Locatario { Id = 1, Nome = "Maria Silva" }
                    }
                }
            };
        }

        private static Pagamento GetTargetPagamento()
        {
            return new Pagamento
            {
                Id = 1,
                Valor = 2000.00m,
                DataPagamento = DateTime.Now.AddDays(-30),
                TipoPagamento = "P",
                Idaluguel = 1,
                IdaluguelNavigation = new Aluguel
                {
                    Id = 1,
                    Idlocatario = 1,
                    Idimovel = 1,
                    Status = "A",
                    IdimovelNavigation = new Imovel
                    {
                        Id = 1,
                        IdLocador = 1,
                        Valor = 2000.00m,
                        Logradouro = "Rua A",
                        Numero = "100",
                        Bairro = "Centro",
                        Cidade = "São Paulo"
                    },
                    IdlocatarioNavigation = new Locatario
                    {
                        Id = 1,
                        Nome = "Maria Silva"
                    }
                }
            };
        }

        private static PagamentoViewModel GetTargetPagamentoModel()
        {
            return new PagamentoViewModel
            {
                Id = 1,
                Valor = 2000.00m,
                TipoPagamento = "P",
                AluguelId = 1,
                DataPagamento = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
                HoraPagamento = TimeOnly.FromDateTime(DateTime.Now.AddDays(-30))
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
                    IdimovelNavigation = new Imovel { Id = 1, IdLocador = 1, Valor = 2000.00m, Logradouro = "Rua A", Cidade = "São Paulo" },
                    IdlocatarioNavigation = new Locatario { Id = 1, Nome = "Maria Silva" }
                },
                new Aluguel {
                    Id = 2, Idlocatario = 2, Idimovel = 2, Status = "A",
                    DataInicio = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)),
                    DataFim = DateOnly.FromDateTime(DateTime.Now.AddMonths(11)),
                    IdimovelNavigation = new Imovel { Id = 2, IdLocador = 1, Valor = 3000.00m, Logradouro = "Rua B", Cidade = "São Paulo" },
                    IdlocatarioNavigation = new Locatario { Id = 2, Nome = "Carlos Santos" }
                }
            };
        }

        private static Locador GetTestLocador()
        {
            return new Locador { Id = 1, Nome = "João Locador", Email = "test@test.com", Cpf = "12345678901" };
        }
    }
}