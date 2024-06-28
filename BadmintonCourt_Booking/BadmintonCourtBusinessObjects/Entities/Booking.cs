using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Booking
{
	public string BookingId { get; set; } = null!;

	public double Amount { get; set; }

	public int BookingType { get; set; }

	public string UserId { get; set; } = null!;

	public DateTime BookingDate { get; set; }

	public int ChangeLog { get; set; }

	public virtual ICollection<BookedSlot> BookedSlots { get; set; } = new List<BookedSlot>();

	public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

	public virtual User User { get; set; } = null!;
}
