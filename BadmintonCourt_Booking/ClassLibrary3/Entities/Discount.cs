using System;
using System.Collections.Generic;

namespace ClassLibrary3.Entities;

public partial class Discount
{
    public string DiscountId { get; set; } = null!;

    public double Amount { get; set; }

    public double Proportion { get; set; }

    public bool? IsDelete { get; set; }
}
