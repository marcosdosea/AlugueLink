using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AlugueLinkWEB.Controllers;
using Core.Service;

namespace AlugueLinkWebTests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController controller;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var mockImovelService = new Mock<IImovelService>();
            var mockLocatarioService = new Mock<ILocatarioService>();
            var mockAluguelService = new Mock<IAluguelService>();
            
            // Setup dos mocks para retornar valores padrão
            mockImovelService.Setup(s => s.GetCount()).Returns(0);
            mockLocatarioService.Setup(s => s.GetCount()).Returns(0);
            mockAluguelService.Setup(s => s.GetCount()).Returns(0);
            mockAluguelService.Setup(s => s.GetImoveisIndisponiveis()).Returns(new List<int>());
            mockAluguelService.Setup(s => s.GetLocatariosOcupados()).Returns(new List<int>());
            
            controller = new HomeController(mockLogger.Object, mockImovelService.Object, 
                mockLocatarioService.Object, mockAluguelService.Object);
            
            // Setup HttpContext para o método Error
            var httpContext = new Mock<HttpContext>();
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