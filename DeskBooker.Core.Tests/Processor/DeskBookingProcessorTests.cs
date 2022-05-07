using System;
using System.Collections.Generic;
using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using Xunit;

namespace DeskBooker.Core.Processor;

public class DeskBookingProcessorTests
{
    private readonly DeskBookingRequest _deskBookingRequest;
    private readonly List<Desk> _availableDesks;
    private readonly DeskBookingProcessor _deskBookingProcessor;
    private readonly Mock<IDeskBookingRepository> _mockDeskBookingRepository;
    private readonly Mock<IDeskRepository> _mockDeskRepository;

    public DeskBookingProcessorTests()
    {
        _deskBookingRequest = new DeskBookingRequest
        {
            FirstName = "Stephen",
            LastName = "Gowen",
            Email = "sjgowen@gmail.com",
            Date = new DateTime(2022, 4, 30)
        };

        _availableDesks = new List<Desk>
        {
            new Desk()
        };

        _mockDeskBookingRepository = new Mock<IDeskBookingRepository>();
        _mockDeskRepository = new Mock<IDeskRepository>();
        _mockDeskRepository.Setup(x => x.GetAvailableDesks(_deskBookingRequest.Date)).Returns(_availableDesks);

        _deskBookingProcessor = new DeskBookingProcessor(_mockDeskBookingRepository.Object, _mockDeskRepository.Object);
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

    [Fact]
    public void ShouldNotSaveDeskBookingIfDeskNotAvailable()
    {
        _availableDesks.Clear();

        _deskBookingProcessor.BookDesk(_deskBookingRequest);

        _mockDeskBookingRepository.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
    }
}
