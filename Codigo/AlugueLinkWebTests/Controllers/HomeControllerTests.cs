using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AlugueLinkWEB.Controllers;
using Core.Service;
using System.Security.Claims;
using System.Security.Principal;

namespace AlugueLinkWebTests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController controller = null!;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var mockImovelService = new Mock<IImovelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockAluguelService = new Mock<IAluguelService>();
            
            // Setup dos mocks para retornar valores padrão
            mockImovelService.Setup(s => s.GetCount()).Returns(10);
            mockLocatarioService.Setup(s => s.GetCount()).Returns(5);
            mockAluguelService.Setup(s => s.GetCount()).Returns(3);
            mockAluguelService.Setup(s => s.GetImoveisIndisponiveis()).Returns(new List<int> { 1, 2 });
            mockAluguelService.Setup(s => s.GetLocatariosOcupados()).Returns(new List<int> { 1 });
            mockAluguelService.Setup(s => s.AtualizarStatusAlugueis()).Verifiable();
            
            controller = new HomeController(mockLogger.Object, mockImovelService.Object, 
                mockLocatarioService.Object, mockAluguelService.Object);
            
            // Setup HttpContext e User para simular usuário não autenticado
            var httpContext = new Mock<HttpContext>();
            var identity = new Mock<IIdentity>();
            identity.Setup(x => x.IsAuthenticated).Returns(false);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Identity).Returns(identity.Object);
            httpContext.Setup(x => x.User).Returns(principal.Object);
            httpContext.Setup(x => x.TraceIdentifier).Returns("test-trace-id");
            
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void IndexTest_UsuarioAutenticado()
        {
            // Arrange - Setup usuário autenticado
            var httpContext = new Mock<HttpContext>();
            var identity = new Mock<IIdentity>();
            identity.Setup(x => x.IsAuthenticated).Returns(true);
            var principal = new Mock<ClaimsPrincipal>();
            principal.Setup(x => x.Identity).Returns(identity.Object);
            httpContext.Setup(x => x.User).Returns(principal.Object);
            
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            
            // Verificar se ViewBag foi populado para usuário autenticado
            Assert.IsNotNull(controller.ViewBag.TotalImoveis);
            Assert.AreEqual(10, controller.ViewBag.TotalImoveis);
        }

        [TestMethod]
        public void PrivacyTest_Valido()
        {
            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void ErrorTest_Valido()
        {
            // Act
            var result = controller.Error();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ErrorViewModel));
            
            var errorModel = (ErrorViewModel)viewResult.ViewData.Model;
            Assert.IsNotNull(errorModel);
            Assert.AreEqual("test-trace-id", errorModel.RequestId);
        }
    }
}