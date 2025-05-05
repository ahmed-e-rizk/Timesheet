using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.DTO.TimesheetLog
{
    public class AttendanceLogsDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }

        public DateTime? LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

    }
}
