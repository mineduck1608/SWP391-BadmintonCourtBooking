using System;
using System.Collections.Generic;

namespace DAO.Entities;

public partial class Booking
{
    public int BookingId { get; set; }

    public double TotalPrice { get; set; }

    public int BookingType { get; set; }

    public int BookingStatus { get; set; }

    public int? UserId { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<CourtActiveSlot> CourtActiveSlots { get; set; } = new List<CourtActiveSlot>();
}
