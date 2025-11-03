using Microsoft.Extensions.Logging;
using Moq;
using Waracle.HotelBookingSystem.Application.CommandHandlers;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;

namespace Waracle.HotelBookingSystem.Application.Tests.CommandHandlers
{
    [TestFixture]
    public class ClearAllTransactionalDataCommandHandlerTests
    {
        private ClearAllTransactionalDataCommandHandler _systemUnderTest;
        private Mock<IHotelsRepository> _mockHotelsRepository;
        private Mock<IBookingsRepository> _mockBookingsRepository;
        private Mock<IRoomsRepository> _mockRoomsRepository;
        private Mock<ILogger<ClearAllTransactionalDataCommandHandler>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockHotelsRepository = new Mock<IHotelsRepository>();
            _mockBookingsRepository = new Mock<IBookingsRepository>();
            _mockRoomsRepository = new Mock<IRoomsRepository>();
            _mockLogger = new Mock<ILogger<ClearAllTransactionalDataCommandHandler>>();
            _systemUnderTest = new ClearAllTransactionalDataCommandHandler(_mockHotelsRepository.Object,
                _mockBookingsRepository.Object, _mockRoomsRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_CallsRemoveAllOnAllRepositories()
        {
            var command = new ClearAllTransactionalDataCommand();

            await _systemUnderTest.Handle(command, CancellationToken.None);

            _mockBookingsRepository.Verify(r => r.RemoveAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRoomsRepository.Verify(r => r.RemoveAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockHotelsRepository.Verify(r => r.RemoveAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_ThrowsOperationCanceledException_WhenCancelled()
        {
            var command = new ClearAllTransactionalDataCommand();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(async () => await _systemUnderTest.Handle(command, cts.Token),
                Throws.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Handle_ThrowsException_WhenRepositoryThrows()
        {
            var command = new ClearAllTransactionalDataCommand();
            _mockBookingsRepository.Setup(r => r.RemoveAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test error"));

            Assert.That(async () => await _systemUnderTest.Handle(command, CancellationToken.None),
                Throws.Exception.With.Message.EqualTo("Test error"));
            _mockLogger.Verify(
                l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }
    }
}