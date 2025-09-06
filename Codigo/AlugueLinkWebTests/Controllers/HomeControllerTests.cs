using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AlugueLinkWEB.Controllers.Tests
{
    [TestClass()]
    public class HomeControllerTests
    {
        private static HomeController controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            controller = new HomeController(mockLogger.Object);
            
            // Setup HttpContext para o método Error
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.TraceIdentifier).Returns("test-trace-id");
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
        }

        [TestMethod()]
        public void IndexTest_Valido()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void PrivacyTest_Valido()
        {
            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
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