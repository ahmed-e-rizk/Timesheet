using System;
using System.Collections.Generic;

namespace Timesheet.Core.Entites;

public partial class AttendanceLogs
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateOnly Date { get; set; }

    public DateTime? LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public virtual User User { get; set; } = null!;
}
