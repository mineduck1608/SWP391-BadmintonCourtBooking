using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.SupportEntities.Court
{
	public class CourtDTO
	{
		public string CourtId { get; set; } = null!;

		public List<string>? CourtImg { get; set; }

		public string BranchId { get; set; } = null!;

		public float Price { get; set; }

		public string Description { get; set; } = null!;

		public string CourtName { get; set; } = null!;

		public bool CourtStatus { get; set; }
	}
}
