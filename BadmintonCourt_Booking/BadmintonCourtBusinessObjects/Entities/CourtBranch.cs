using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class CourtBranch 
{
	public string BranchId { get; set; }

	public string Location { get; set; }

	public string BranchName { get; set; }

	public string BranchPhone { get; set; }

	public string? BranchImg { get; set; }

	public int BranchStatus { get; set; }

	public virtual ICollection<Court> Courts { get; set; } = new List<Court>();

	public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

	public virtual ICollection<User> Users { get; set; } = new List<User>();
}
