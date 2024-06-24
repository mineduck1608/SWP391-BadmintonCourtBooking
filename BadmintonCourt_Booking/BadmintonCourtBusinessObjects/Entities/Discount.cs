using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Discount
{
    public string DiscountId { get; set; } = null!;

    public double Amount { get; set; }

    public double Proportion { get; set; }
}
