using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.SupportEntities.Slot
{
	public class BookedSlotSchema
	{
		public DateOnly Date {  get; set; }
		public int Start { get; set; }
		public int End { get; set; }
		public string BookingId { get; set; }
		public string BookedSlotId { get; set; }

	}
}
