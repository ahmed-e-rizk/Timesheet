using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.BaseResponse;
using Timesheet.Core.Entites;
using Timesheet.DTO;
using Timesheet.DTO.TimesheetLog;

namespace Timesheet.BLL.TimesheetLog
{
    public interface ITimesheetAttendanceBLL
    {
        Task<IResponse<AttendanceLogsDto>> GetTodayAttendance(int userId);
        Task<IResponse<PagedResultDto<AttendanceLogsDto>>> GetAllAttendance(int userId,int page=0, int size=20);
        Task<IResponse<bool>> SubmitPunchOutTime(int userId ,DateTime? PunchOut=null);
        Task<IResponse<bool>> SubmitPunchInTime(int userId ,DateTime? PunchIn=null);
    }
}
