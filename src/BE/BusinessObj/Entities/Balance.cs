using System;
using System.Collections.Generic;

namespace DAO.Entities;

public partial class Balance
{
    public int UserId { get; set; }

    public double Balance1 { get; set; }

    public virtual User User { get; set; } = null!;
}
