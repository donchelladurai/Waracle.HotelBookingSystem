using Microsoft.Extensions.Logging;
using Moq;
using Waracle.HotelBookingSystem.Application.CommandHandlers;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.Tests.CommandHandlers
{
    [TestFixture]
    public class SeedDataCommandHandlerTests
    {
        private SeedDataCommandHandler _systemUnderTest;
        private Mock<IHotelsRepository> _mockHotelsRepository;
        private Mock<ILogger<SeedDataCommandHandler>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockHotelsRepository = new Mock<IHotelsRepository>();
            _mockLogger = new Mock<ILogger<SeedDataCommandHandler>>();
            _systemUnderTest = new SeedDataCommandHandler(_mockHotelsRepository.Object, _mockLogger.Object);
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenHotelsRepositoryIsNull()
        {
            Assert.That(() => new SeedDataCommandHandler(null, _mockLogger.Object),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            Assert.That(() => new SeedDataCommandHandler(_mockHotelsRepository.Object, null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task Handle_ThrowsArgumentNullException_WhenCommandIsNull()
        {
            Assert.That(async () => await _systemUnderTest.Handle(null, CancellationToken.None),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task Handle_ReturnsFalse_WhenDatabaseIsNotEmpty()
        {
            var command = new SeedDataCommand();
            _mockHotelsRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Hotel> { new Hotel() });

            var result = await _systemUnderTest.Handle(command, CancellationToken.None);

            Assert.That(result, Is.False);
            _mockHotelsRepository.Verify(r => r.CreateAsync(It.IsAny<List<Hotel>>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _mockLogger.Verify(
                l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true), null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Handle_SeedsDataAndReturnsTrue_WhenDatabaseIsEmpty()
        {
            var command = new SeedDataCommand();
            _mockHotelsRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Hotel>());

            var result = await _systemUnderTest.Handle(command, CancellationToken.None);

            Assert.That(result, Is.True);
            _mockHotelsRepository.Verify(
                r => r.CreateAsync(It.Is<List<Hotel>>(h => h.Count == 5 && h.All(ht => ht.Rooms.Count == 6)),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_ThrowsOperationCanceledException_WhenCancelled()
        {
            var command = new SeedDataCommand();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(async () => await _systemUnderTest.Handle(command, cts.Token),
                Throws.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Handle_ThrowsException_WhenRepositoryThrows()
        {
            var command = new SeedDataCommand();
            _mockHotelsRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test error"));

            Assert.That(async () => await _systemUnderTest.Handle(command, CancellationToken.None),
                Throws.Exception.With.Message.EqualTo("Test error"));
            _mockLogger.Verify(
                l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }
    }
}