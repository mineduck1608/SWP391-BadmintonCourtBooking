using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Booking
{
	public string BookingId { get; set; } = null!;

	public double Amount { get; set; }

	public int BookingType { get; set; }

	public string UserId { get; set; } = null!;

	public virtual Payment? Payment { get; set; }

	public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

	public virtual User User { get; set; } = null!;
}
