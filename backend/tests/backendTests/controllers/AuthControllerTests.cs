using backend;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;

namespace backendTests.controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var factory = new WebApplicationFactory<Startup>();
            _client = factory.CreateClient();
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var content = new StringContent(@"{ ""username"": ""admin"", ""password"": ""password"" }", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var token = await response.Content.ReadAsStringAsync();
            Assert.That(token, Is.Not.Null.Or.Empty);
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var content = new StringContent(@"{ ""username"": ""invalid"", ""password"": ""invalid"" }", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}
