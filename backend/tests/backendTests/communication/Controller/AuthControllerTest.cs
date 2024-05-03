using backend.communication.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace backendTests.communication.Controller
{
    internal class AuthControllerTest
    {
        [Test]
        public void CheckAuthentication_ReturnsUnauthorized_WhenTokenIsNotPresent()
        {
            // Arrange
            var authController = new AuthController();
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c["authToken"]).Returns((string)null);
            requestMock.SetupGet(r => r.Cookies).Returns(cookiesMock.Object);
            httpContextMock.SetupGet(c => c.Request).Returns(requestMock.Object);
            authController.ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object };

            // Act
            var result = authController.CheckAuthentication() as UnauthorizedResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CheckAuthentication_ReturnsOk_WhenTokenIsPresent()
        {
            // Arrange
            var authController = new AuthController();
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c["authToken"]).Returns("fakeAuthToken");
            requestMock.SetupGet(r => r.Cookies).Returns(cookiesMock.Object);
            httpContextMock.SetupGet(c => c.Request).Returns(requestMock.Object);
            authController.ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object };

            // Act
            var result = authController.CheckAuthentication() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue((bool)result.Value.GetType().GetProperty("authenticated").GetValue(result.Value, null));

        }
    }
}
