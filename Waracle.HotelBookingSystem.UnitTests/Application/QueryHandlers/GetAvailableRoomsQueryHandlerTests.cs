using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Application.QueryHandlers;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Common.Helpers;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.Tests.QueryHandlers
{
    [TestFixture]
    public class GetAvailableRoomsQueryHandlerTests
    {
        private GetAvailableRoomsQueryHandler _systemUnderTest;
        private Mock<IRoomsRepository> _roomsRepositoryMock;
        private Mock<ILogger<GetAvailableRoomsQueryHandler>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _roomsRepositoryMock = new Mock<IRoomsRepository>();
            _loggerMock = new Mock<ILogger<GetAvailableRoomsQueryHandler>>();
            _systemUnderTest = new GetAvailableRoomsQueryHandler(_roomsRepositoryMock.Object, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _systemUnderTest = null;
            _roomsRepositoryMock = null;
            _loggerMock = null;
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenRoomsRepositoryIsNull()
        {
            Assert.That(() => new GetAvailableRoomsQueryHandler(null, _loggerMock.Object), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            Assert.That(() => new GetAvailableRoomsQueryHandler(_roomsRepositoryMock.Object, null), Throws.ArgumentNullException);
        }

        [Test]
        public void Handle_ThrowsArgumentNullException_WhenRequestIsNull()
        {
            Assert.That(() => _systemUnderTest.Handle(null, CancellationToken.None), Throws.ArgumentNullException);
        }

        [Test]
        public void Handle_ThrowsArgumentException_WhenNumberOfGuestsIsZero()
        {
            var request = new GetAvailableRoomsQuery(DateTime.Now, DateTime.Now.AddDays(1), 0);
            Assert.That(async () => await _systemUnderTest.Handle(request, CancellationToken.None), Throws.ArgumentException);
        }

        [Test]
        public void Handle_ThrowsArgumentException_WhenNumberOfGuestsIsNegative()
        {
            var request = new GetAvailableRoomsQuery(DateTime.Now, DateTime.Now.AddDays(1), -100);
            Assert.That(async () => await _systemUnderTest.Handle(request, CancellationToken.None), Throws.ArgumentException);
        }

        [Test]
        public async Task Handle_ReturnsAvailableRooms_WhenRoomsAreAvailable()
        {
            var request = new GetAvailableRoomsQuery(DateTime.Now, DateTime.Now.AddDays(1), 2);

            var rooms = new List<Room>
            {
                new Room
                {
                    Id = 1,
                    HotelId = 1,
                    Hotel = new Hotel { Name = "Hotel1" },
                    RoomType = new RoomType { Name = "Standard", Capacity = 2 },
                    Bookings = new List<Booking>()
                }
            };

            _roomsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(rooms);

            var result = await _systemUnderTest.Handle(request, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().RoomId, Is.EqualTo(1));
            Assert.That(result.First().HotelName, Is.EqualTo("Hotel1"));
            Assert.That(result.First().RoomType, Is.EqualTo("Standard"));
            Assert.That(result.First().Capacity, Is.EqualTo(2));
        }

        [Test]
        public void Handle_ThrowsOperationCanceledException_WhenCancelled()
        {
            var request = new GetAvailableRoomsQuery(DateTime.Now, DateTime.Now.AddDays(1), 2);

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(async () => await _systemUnderTest.Handle(request, cts.Token), Throws.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Handle_ThrowsException_WhenRepositoryThrowsException()
        {
            var request = new GetAvailableRoomsQuery(DateTime.Now, DateTime.Now.AddDays(1), 2);

            _roomsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

            Assert.That(async () => await _systemUnderTest.Handle(request, CancellationToken.None), Throws.Exception);
        }
    }

    [TestFixture]
    public class GetHotelsByNameQueryHandlerTests
    {
        private GetHotelsByNameQueryHandler _systemUnderTest;
        private Mock<IHotelsRepository> _hotelsRepositoryMock;
        private Mock<ILogger<GetHotelsByNameQueryHandler>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _hotelsRepositoryMock = new Mock<IHotelsRepository>();
            _loggerMock = new Mock<ILogger<GetHotelsByNameQueryHandler>>();
            _systemUnderTest = new GetHotelsByNameQueryHandler(_hotelsRepositoryMock.Object, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _systemUnderTest = null;
            _hotelsRepositoryMock = null;
            _loggerMock = null;
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenHotelsRepositoryIsNull()
        {
            Assert.That(() => new GetHotelsByNameQueryHandler(null, _loggerMock.Object), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            Assert.That(() => new GetHotelsByNameQueryHandler(_hotelsRepositoryMock.Object, null), Throws.ArgumentNullException);
        }

        [Test]
        public void Handle_ThrowsArgumentNullException_WhenRequestIsNull()
        {
            Assert.That(() => _systemUnderTest.Handle(null, CancellationToken.None), Throws.ArgumentNullException);
        }

        [Test]
        public void Handle_ThrowsArgumentNullException_WhenNameIsNull()
        {
            var request = new GetHotelsByNameQuery(null);
            Assert.That(async () => await _systemUnderTest.Handle(request, CancellationToken.None), Throws.ArgumentNullException);
        }

        [Test]
        public async Task Handle_ReturnsHotelDtos_WhenHotelsExist()
        {
            var request = new GetHotelsByNameQuery("Hotel1");

            var hotels = new List<Hotel>
            {
                new Hotel { Id = 1, Name = "Hotel1" }
            };

            _hotelsRepositoryMock.Setup(r => r.GetByNameAsync("Hotel1", It.IsAny<CancellationToken>())).ReturnsAsync(hotels);

            var result = await _systemUnderTest.Handle(request, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Hotel1"));
        }

        [Test]
        public async Task Handle_ReturnsEmpty_WhenNoHotelsExist()
        {
            var request = new GetHotelsByNameQuery("Hotel1");

            _hotelsRepositoryMock.Setup(r => r.GetByNameAsync("Hotel1", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Hotel>());

            var result = await _systemUnderTest.Handle(request, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Handle_ThrowsOperationCanceledException_WhenCancelled()
        {
            var request = new GetHotelsByNameQuery("Hotel1");

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(async () => await _systemUnderTest.Handle(request, cts.Token), Throws.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Handle_ThrowsException_WhenRepositoryThrowsException()
        {
            var request = new GetHotelsByNameQuery("Hotel1");

            _hotelsRepositoryMock.Setup(r => r.GetByNameAsync("Hotel1", It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

            Assert.That(async () => await _systemUnderTest.Handle(request, CancellationToken.None), Throws.Exception);
        }
    }

    [TestFixture]
    public class GetAllBookingsQueryHandlerTests
    {
        private GetAllBookingsQueryHandler _systemUnderTest;
        private Mock<IBookingsRepository> _bookingsRepositoryMock;
        private Mock<ILogger<GetAllBookingsQueryHandler>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _bookingsRepositoryMock = new Mock<IBookingsRepository>();
            _loggerMock = new Mock<ILogger<GetAllBookingsQueryHandler>>();
            _systemUnderTest = new GetAllBookingsQueryHandler(_bookingsRepositoryMock.Object, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _systemUnderTest = null;
            _bookingsRepositoryMock = null;
            _loggerMock = null;
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenBookingsRepositoryIsNull()
        {
            Assert.That(() => new GetAllBookingsQueryHandler(null, _loggerMock.Object), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            Assert.That(() => new GetAllBookingsQueryHandler(_bookingsRepositoryMock.Object, null), Throws.ArgumentNullException);
        }

        [Test]
        public async Task Handle_ReturnsAllBookings_WhenBookingsExist()
        {
            var request = new GetAllBookingsQuery();

            var bookings = new List<Booking>
            {
                new Booking
                {
                    Reference = "REF123",
                    Room = new Room
                    {
                        HotelId = 1,
                        Id = 1,
                        Hotel = new Hotel { Name = "Hotel1" },
                        RoomType = new RoomType { Name = "Standard" }
                    },
                    CheckInDate = DateTime.Now,
                    CheckOutDate = DateTime.Now.AddDays(1),
                    NumberOfGuests = 2
                }
            };

            _bookingsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

            var result = await _systemUnderTest.Handle(request, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().BookingReference, Is.EqualTo("REF123"));
            Assert.That(result.First().HotelName, Is.EqualTo("Hotel1"));
            Assert.That(result.First().RoomType, Is.EqualTo("Standard"));
            Assert.That(result.First().NumberOfGuests, Is.EqualTo(2));
        }

        [Test]
        public async Task Handle_ReturnsEmpty_WhenNoBookingsExist()
        {
            var request = new GetAllBookingsQuery();

            _bookingsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Booking>());

            var result = await _systemUnderTest.Handle(request, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Handle_ThrowsException_WhenRepositoryThrowsException()
        {
            var request = new GetAllBookingsQuery();

            _bookingsRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

            Assert.That(async () => await _systemUnderTest.Handle(request, CancellationToken.None), Throws.Exception);
        }
    }
}