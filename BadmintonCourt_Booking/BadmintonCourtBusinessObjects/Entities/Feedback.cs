using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Feedback 
{
	public string FeedbackId { get; set; }

	public int Rate { get; set; }

	public string Content { get; set; }

	public string UserId { get; set; }

	public string BranchId { get; set; }

	public virtual CourtBranch Branch { get; set; }

	public virtual User? User { get; set; }

}
