using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.Entities
{
	public class Discount
	{
		public string DiscountId { get; set; } = null!;

		public double Amount { get; set; }

		public double Proportion { get; set; }
	}
}
