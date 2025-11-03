using Waracle.HotelBookingSystem.Application.Commands;

namespace Waracle.HotelBookingSystem.Application.Tests.Commands
{
    [TestFixture]
    public class BookRoomCommandTests
    {
        [Test]
        public void IsCheckoutDateAfterCheckInDate_ReturnsTrue_WhenCheckoutAfterCheckin()
        {
            var command = new BookRoomCommand(1, 1, DateTime.Now, DateTime.Now.AddDays(1), 1);

            var result = command.IsCheckoutDateAfterCheckInDate();

            Assert.That(result, Is.True);
        }
    }
}