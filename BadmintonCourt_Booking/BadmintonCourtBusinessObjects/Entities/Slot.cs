using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Slot 
{
    public int SlotId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public bool Status { get; set; }

    public int CourtId { get; set; }

    public int? BookingId { get; set; }

    public virtual Booking? Booking { get; set; } = null!;

    public virtual Court Court { get; set; } = null!;

    public Slot(int slotId, DateTime startTime, DateTime endTime, bool status, int courtId, int? bookingId)
    {
        this.SlotId = slotId;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
        CourtId = courtId;
        BookingId = bookingId;
    }
}
