using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.SupportEntities.Court
{
	public class BranchDTO
	{
		public string BranchId { get; set; } = null!;

		public string Location { get; set; } = null!;

		public string? MapUrl { get; set; } = null!;

		public string BranchName { get; set; } = null!;

		public string BranchPhone { get; set; } = null!;

		public List<string>? BranchImg { get; set; }

		public int BranchStatus { get; set; }
	}
}
