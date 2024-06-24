using System;
using System.Collections.Generic;

namespace ClassLibrary1.Entities;

public partial class Booking
{
    public int BookingId { get; set; }

    public double TotalPrice { get; set; }

    public int BookingType { get; set; }

    public int BookingStatus { get; set; }

    public int? UserId { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public virtual User? User { get; set; }
}
