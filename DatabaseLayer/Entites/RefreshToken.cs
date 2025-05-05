using System;
using System.Collections.Generic;

namespace Timesheet.Core.Entites;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Jti { get; set; }

    public string? Token { get; set; }

    public DateTime? ExpireDate { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual User? User { get; set; }
}
