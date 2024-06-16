using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Court 
{
	public string CourtId { get; set; }

	public string? CourtImg { get; set; }

	public string BranchId { get; set; }

	public float Price { get; set; }

	public string Description { get; set; }

	public bool CourtStatus { get; set; }

	public virtual CourtBranch Branch { get; set; }

	public virtual ICollection<Slot> Slots { get; set; }

}
