using Microsoft.Extensions.Logging;
using Moq;
using Waracle.HotelBookingSystem.Application.CommandHandlers;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.Tests.CommandHandlers
{
    [TestFixture]
    public class BookRoomCommandHandlerTests
    {
        private BookRoomCommandHandler _systemUnderTest;
        private Mock<IBookingsRepository> _mockBookingsRepository;
        private Mock<IRoomsRepository> _mockRoomsRepository;
        private Mock<IHotelsRepository> _mockHotelsRepository;
        private Mock<ILogger<BookRoomCommandHandler>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockBookingsRepository = new Mock<IBookingsRepository>();
            _mockRoomsRepository = new Mock<IRoomsRepository>();
            _mockHotelsRepository = new Mock<IHotelsRepository>();
            _mockLogger = new Mock<ILogger<BookRoomCommandHandler>>();
            _systemUnderTest = new BookRoomCommandHandler(_mockBookingsRepository.Object, _mockRoomsRepository.Object,
                _mockHotelsRepository.Object, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // No disposables
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenBookingsRepositoryIsNull()
        {
            Assert.That(
                () => new BookRoomCommandHandler(null, _mockRoomsRepository.Object, _mockHotelsRepository.Object,
                    _mockLogger.Object), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenRoomsRepositoryIsNull()
        {
            Assert.That(
                () => new BookRoomCommandHandler(_mockBookingsRepository.Object, null, _mockHotelsRepository.Object,
                    _mockLogger.Object), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenHotelsRepositoryIsNull()
        {
            Assert.That(
                () => new BookRoomCommandHandler(_mockBookingsRepository.Object, _mockRoomsRepository.Object, null,
                    _mockLogger.Object), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            Assert.That(
                () => new BookRoomCommandHandler(_mockBookingsRepository.Object, _mockRoomsRepository.Object,
                    _mockHotelsRepository.Object, null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task Handle_ThrowsArgumentNullException_WhenCommandIsNull()
        {
            Assert.That(async () => await _systemUnderTest.Handle(null, CancellationToken.None),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task Handle_ThrowsArgumentOutOfRangeException_WhenNumberOfGuestsIsZeroOrNegative()
        {
            var command = new BookRoomCommand(1, 1, DateTime.Now, DateTime.Now.AddDays(1), 0);

            Assert.That(async () => await _systemUnderTest.Handle(command, CancellationToken.None),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public async Task Handle_ThrowsArgumentException_WhenCheckOutDateNotAfterCheckInDate()
        {
            var command = new BookRoomCommand(1, 1, DateTime.Now, DateTime.Now, 1);

            Assert.That(async () => await _systemUnderTest.Handle(command, CancellationToken.None),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public async Task Handle_ThrowsArgumentException_WhenHotelDoesNotExist()
        {
            var command = new BookRoomCommand(1, 1, DateTime.Now, DateTime.Now.AddDays(1), 1);
            _mockHotelsRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Hotel)null);

            Assert.That(async () => await _systemUnderTest.Handle(command, CancellationToken.None),
                Throws.TypeOf<ArgumentException>().With.Message.Contains("HotelId"));
        }

        [Test]
        public async Task Handle_ThrowsArgumentException_WhenRoomDoesNotExistInHotel()
        {
            var command = new BookRoomCommand(1, 999, DateTime.Now, DateTime.Now.AddDays(1), 1);
            var hotel = new Hotel { Id = 1, Rooms = new List<Room> { new Room { Id = 1 } } };
            _mockHotelsRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(hotel);

            Assert.That(async () => await _systemUnderTest.Handle(command, CancellationToken.None),
                Throws.TypeOf<ArgumentException>().With.Message.Contains("RoomId"));
        }

        [Test]
        public void Handle_ThrowsOperationCanceledException_WhenCancelled()
        {
            var command = new BookRoomCommand(1, 1, DateTime.Now, DateTime.Now.AddDays(1), 1);
            var cts = new CancellationTokenSource();
            cts.Cancel();
            _mockHotelsRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Hotel { Rooms = new List<Room> { new Room { Id = 1 } } });

            Assert.That(async () => await _systemUnderTest.Handle(command, cts.Token),
                Throws.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Handle_ThrowsException_WhenRepositoryThrows()
        {
            var command = new BookRoomCommand(1, 1, DateTime.Now, DateTime.Now.AddDays(1), 1);
            _mockHotelsRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Hotel { Rooms = new List<Room> { new Room { Id = 1 } } });
            _mockRoomsRepository.Setup(r => r.GetByHotelIdAsync(1, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test error"));

            Assert.That(async () => await _systemUnderTest.Handle(command, CancellationToken.None),
                Throws.Exception.With.Message.EqualTo("Test error"));
        }
    }
}