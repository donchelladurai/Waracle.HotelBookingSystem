using Microsoft.Extensions.Logging;
using Moq;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Application.QueryHandlers;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.UnitTests.Application.QueryHandlers
{
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
            Assert.That(() => new GetHotelsByNameQueryHandler(_hotelsRepositoryMock.Object, null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void Handle_ThrowsArgumentNullException_WhenRequestIsNull()
        {
            Assert.That(() => _systemUnderTest.Handle(null, CancellationToken.None), Throws.ArgumentNullException);
        }

        [Test]
        public async Task Handle_ReturnsHotelDtos_WhenHotelsExist()
        {
            var request = new GetHotelsByNameQuery("Hotel1");

            var hotels = new List<Hotel>
            {
                new Hotel { Id = 1, Name = "Hotel1" }
            };

            _hotelsRepositoryMock.Setup(r => r.GetByNameAsync("Hotel1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(hotels);

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

            _hotelsRepositoryMock.Setup(r => r.GetByNameAsync("Hotel1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Hotel>());

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

            Assert.That(async () => await _systemUnderTest.Handle(request, cts.Token),
                Throws.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Handle_ThrowsException_WhenRepositoryThrowsException()
        {
            var request = new GetHotelsByNameQuery("Hotel1");

            _hotelsRepositoryMock.Setup(r => r.GetByNameAsync("Hotel1", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            Assert.That(async () => await _systemUnderTest.Handle(request, CancellationToken.None), Throws.Exception);
        }
    }
}
