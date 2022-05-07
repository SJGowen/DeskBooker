using System;
using System.Linq;
using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;

namespace DeskBooker.Core.Processor;

public class DeskBookingProcessor
{
    private readonly IDeskBookingRepository _deskBookingRepository;
    private readonly IDeskRepository _deskRepository;

    public DeskBookingProcessor(IDeskBookingRepository deskBookingRepository, IDeskRepository deskRepository)
    {
        _deskBookingRepository = deskBookingRepository;
        _deskRepository = deskRepository;
    }

    public DeskBookingResult BookDesk(DeskBookingRequest? request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var result = Create<DeskBookingResult>(request);

        var availableDesks = _deskRepository.GetAvailableDesks(request.Date);
        if (availableDesks.FirstOrDefault() is Desk availableDesk)
        {
            var deskBooking = Create<DeskBooking>(request);
            deskBooking.DeskID = availableDesk.ID;
            _deskBookingRepository.Save(deskBooking);
            result.DeskBookingID = deskBooking.ID;
            result.Code = DeskBookingResultCode.Success;
        }
        else
        {
            result.Code = DeskBookingResultCode.NoDeskAvailable;
        }
        

        return result;
    }

    private static T Create<T>(DeskBookingRequest request) where T : DeskBookingBase, new()
    {
        return new T
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Date = request.Date
        };
    }
}
