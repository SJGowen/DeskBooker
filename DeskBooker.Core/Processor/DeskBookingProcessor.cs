using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using System;

namespace DeskBooker.Core.Processor;

public class DeskBookingProcessor
{
    private readonly IDeskBookingRepository _deskBookingRepository;

    public DeskBookingProcessor(IDeskBookingRepository deskBookingRepository)
    {
        _deskBookingRepository = deskBookingRepository;
    }

    public DeskBookingResult BookDesk(DeskBookingRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _deskBookingRepository.Save(new DeskBooking
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Date = request.Date
        });

        return new DeskBookingResult
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Date = request.Date
        };
    }
}
