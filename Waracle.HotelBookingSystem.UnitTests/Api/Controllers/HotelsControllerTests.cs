using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Web.Api.Controllers;

namespace Waracle.HotelBookingSystem.UnitTests.Api.Controllers
{
    [TestFixture]
    public class HotelsControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<HotelsController>> _loggerMock;
        private HotelsController _systemUnderTest;

        [TearDown]
        public void TearDown()
        {
            _systemUnderTest.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<HotelsController>>();
            _systemUnderTest = new HotelsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetByNameAsync_ValidName_ReturnsOkWithHotels()
        {
            // Arrange
            var name = "TestHotel";
            var hotels = new List<HotelDto> { new HotelDto { Id = 1, Name = "TestHotel" } };
            _mediatorMock.Setup(m => m.Send(It.Is<GetHotelsByNameQuery>(q => q.Name == name), default))
                .ReturnsAsync(hotels);

            // Act
            var result = await _systemUnderTest.GetByNameAsync(name, default);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(hotels));
        }

        [Test]
        public async Task GetByNameAsync_EmptyName_ReturnsBadRequest()
        {
            // Arrange
            var name = string.Empty;

            // Act
            var result = await _systemUnderTest.GetByNameAsync(name, default);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetByNameAsync_NoHotelsFound_ReturnsNotFound()
        {
            // Arrange
            var name = "NonExistent";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetHotelsByNameQuery>(), default))
                .ReturnsAsync(new List<HotelDto>());

            // Act
            var result = await _systemUnderTest.GetByNameAsync(name, default);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetByNameAsync_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var name = "Test";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetHotelsByNameQuery>(), default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _systemUnderTest.GetByNameAsync(name, default);
            
            var os = result as ObjectResult;

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetByNameAsync_CancellationRequested_Returns499()
        {
            // Arrange
            var name = "Test";
            var cts = new CancellationTokenSource();
            cts.Cancel();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetHotelsByNameQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act
            var result = await _systemUnderTest.GetByNameAsync(name, cts.Token);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(499));
        }
    }
}