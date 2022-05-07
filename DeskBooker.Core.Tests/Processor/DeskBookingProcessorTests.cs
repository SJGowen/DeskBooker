using System;
using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using Xunit;

namespace DeskBooker.Core.Processor;

public class DeskBookingProcessorTests
{
    private readonly DeskBookingRequest _deskBookingRequest;
    private readonly DeskBookingProcessor _deskBookingProcessor;
    private readonly Mock<IDeskBookingRepository> _mockDeskBookingRepository;

    public DeskBookingProcessorTests()
    {
        _deskBookingRequest = new DeskBookingRequest
        {
            FirstName = "Stephen",
            LastName = "Gowen",
            Email = "sjgowen@gmail.com",
            Date = new DateTime(2022, 4, 30)
        };

        _mockDeskBookingRepository = new Mock<IDeskBookingRepository>();
        _deskBookingProcessor = new DeskBookingProcessor(_mockDeskBookingRepository.Object);
    }

    [Fact]
    public void ShouldReturnDeskBookingResultWithRequestValues()
    {
        DeskBookingResult result = _deskBookingProcessor.BookDesk(_deskBookingRequest);

        Assert.NotNull(result);
        Assert.Equal(_deskBookingRequest.FirstName, result.FirstName);
        Assert.Equal(_deskBookingRequest.LastName, result.LastName);
        Assert.Equal(_deskBookingRequest.Email, result.Email);
        Assert.Equal(_deskBookingRequest.Date, result.Date);
    }

    [Fact]
    public void ShouldThrowExceptionIfRequestIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => _deskBookingProcessor.BookDesk(null));

        Assert.Equal("request", exception.ParamName);
    }

    [Fact]
    public void ShouldSaveDeskBooking()
    {
        DeskBooking savedDeskBooking = null;
        _mockDeskBookingRepository.Setup(x => x.Save(It.IsAny<DeskBooking>())).
            Callback<DeskBooking>(deskBooking =>
            {
                savedDeskBooking = deskBooking;
            });

        _deskBookingProcessor.BookDesk(_deskBookingRequest);

        _mockDeskBookingRepository.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);

        Assert.NotNull(savedDeskBooking);
        Assert.Equal(_deskBookingRequest.FirstName, savedDeskBooking.FirstName);
        Assert.Equal(_deskBookingRequest.LastName, savedDeskBooking.LastName);
        Assert.Equal(_deskBookingRequest.Email, savedDeskBooking.Email);
        Assert.Equal(_deskBookingRequest.Date, savedDeskBooking.Date);

    }
}
