using Xunit;
using Moq;
using TaskManagerAPI.Controllers;
using TaskManagerAPI.Services;
using TaskManagerAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TaskManagerAPI.Tests
{
    public class ProjectControllerTests
    {
        private readonly Mock<IProjectService> _mockProjectService;
        private readonly ProjectController _controller;

        public ProjectControllerTests()
        {
            _mockProjectService = new Mock<IProjectService>();
            _controller = new ProjectController(_mockProjectService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetProjectById_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            // Arrange
            _mockProjectService.Setup(s => s.GetProjectByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((ProjectReadDto?)null);

            // Act
            var result = await _controller.GetProjectById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}