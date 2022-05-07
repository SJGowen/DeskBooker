using System.Collections.Generic;

namespace DeskBooker.Core.Domain;

public class DeskBooking : DeskBookingBase
{
    public int ID { get; set; }
    public int DeskID { get; set; }
}