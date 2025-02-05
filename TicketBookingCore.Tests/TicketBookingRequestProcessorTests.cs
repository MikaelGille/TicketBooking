using Moq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

namespace TicketBookingCore.Tests
{
    public class TicketBookingRequestProcessorTests
    {
        private readonly Mock<ITicketBookingRepository> _ticketBookingRepositoryMock;
        private readonly TicketBookingRequestProcessor _processor;
        public TicketBookingRequestProcessorTests()
        {
            _ticketBookingRepositoryMock = new Mock<ITicketBookingRepository>();
            _processor = new TicketBookingRequestProcessor(_ticketBookingRepositoryMock.Object);
        }

        [Fact]
        public void ShouldReturnTicketBookingResultWithRequestValues()
        {
            // Arrange
            var request = CreateBookingRequest();

            // Act
            var response = _processor.Book(request);

            // Assert
            BookingMatchesRequest(response,  request);
        }

        [Fact]
        public void ShouldThrowExceptionIfRequestIsNull()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => _processor.Book(null));

            // Assert
            Assert.Equal("request", exception.ParamName);
        }

        [Fact]
        public void ShouldSaveToDatabase()
        {
            // Arrange
            TicketBooking savedBooking = null;

            _ticketBookingRepositoryMock
                .Setup(repo => repo.Save(It.IsAny<TicketBooking>()))
                .Callback<TicketBooking>(booking => savedBooking = booking);

            var request = CreateBookingRequest();

            // Act
            var response = _processor.Book(request);

            // Assert
            Assert.NotNull(savedBooking);
            BookingMatchesRequest(savedBooking, request);
        }

        private TicketBookingRequest CreateBookingRequest() => new()
        {
            FirstName = "Förnamn",
            LastName = "Efternamn",
            Email = "email@email.com"
        };

        private void BookingMatchesRequest(TicketBookingBase booking, TicketBookingRequest request)
        {
            Assert.Equal(request.FirstName, booking.FirstName);
            Assert.Equal(request.LastName, booking.LastName);
            Assert.Equal(request.Email, booking.Email);
        }
    }
}