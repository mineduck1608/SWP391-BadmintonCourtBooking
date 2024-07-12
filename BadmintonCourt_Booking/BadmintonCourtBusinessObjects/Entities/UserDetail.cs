using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class UserDetail
{
    public string UserId { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [EmailAddress]
    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Facebook { get; set; }

    public string? Img { get; set; }

    public virtual User User { get; set; } = null!;
}
