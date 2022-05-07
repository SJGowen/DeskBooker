using System;
using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using Xunit;

namespace DeskBooker.Core.Processor;

public class DeskBookingProcessorTests
{
    private readonly DeskBookingProcessor _deskBookingProcessor;
    private readonly Mock<IDeskBookingRepository> _mockDeskBookingRepository;

    public DeskBookingProcessorTests()
    {
        _mockDeskBookingRepository = new Mock<IDeskBookingRepository>();
        _deskBookingProcessor = new DeskBookingProcessor(_mockDeskBookingRepository.Object);
    }

    [Fact]
    public void ShouldReturnDeskBookingResultWithRequestValues()
    {
        // Arrange
        var request = new DeskBookingRequest
        {
            FirstName = "Stephen",
            LastName = "Gowen",
            Email = "sjgowen@gmail.com",
            Date = new DateTime(2022, 4, 30)
        };

        // Act
        DeskBookingResult result = _deskBookingProcessor.BookDesk(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.FirstName, result.FirstName);
        Assert.Equal(request.LastName, result.LastName);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Date, result.Date);
    }

    [Fact]
    public void ShouldThrowExceptionIfRequestIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => _deskBookingProcessor.BookDesk(null));

        Assert.Equal("request", exception.ParamName);
    }

    [Fact]
    public void ShouldSaveBooking()
    {
        var request = new DeskBookingRequest
        {
            FirstName = "Stephen",
            LastName = "Gowen",
            Email = "sjgowen@gmail.com",
            Date = new DateTime(2022, 4, 30)
        };

        DeskBooking savedDeskBooking = null;
        _mockDeskBookingRepository.Setup(x => x.Save(It.IsAny<DeskBooking>())).
            Callback<DeskBooking>(deskBooking =>
            {
                savedDeskBooking = deskBooking;
            });

        _deskBookingProcessor.BookDesk(request);

        _mockDeskBookingRepository.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);

        Assert.NotNull(savedDeskBooking);
        Assert.Equal(request.FirstName, savedDeskBooking.FirstName);
        Assert.Equal(request.LastName, savedDeskBooking.LastName);
        Assert.Equal(request.Email, savedDeskBooking.Email);
        Assert.Equal(request.Date, savedDeskBooking.Date);

    }
}
