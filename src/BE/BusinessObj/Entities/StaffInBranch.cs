using System;
using System.Collections.Generic;

namespace DAO.Entities;

public partial class StaffInBranch
{
    public int UserId { get; set; }

    public int? BranchId { get; set; }

    public virtual CourtBranch? Branch { get; set; }

    public virtual User User { get; set; } = null!;
}
