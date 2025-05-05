using System;
using System.Collections.Generic;

namespace Timesheet.Core.Entites;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<AttendanceLogs> Timesheets { get; set; } = new List<AttendanceLogs>();
}
