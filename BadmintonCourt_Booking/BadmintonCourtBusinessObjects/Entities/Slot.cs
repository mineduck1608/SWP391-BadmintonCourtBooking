using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Slot 
{
	public string SlotId { get; set; }

	public DateTime StartTime { get; set; }

	public DateTime EndTime { get; set; }

	public bool Status { get; set; }

	public string CourtId { get; set; }

	public string BookingId { get; set; }

	public virtual Booking? Booking { get; set; }

	public virtual Court Court { get; set; }

}
