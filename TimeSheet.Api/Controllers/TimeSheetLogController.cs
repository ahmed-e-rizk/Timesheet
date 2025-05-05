using AutoMapper;
using BLL.Auth;
using DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timesheet.BLL.TimesheetLog;
using Timesheet.Controllers;
using Timesheet.DTO.TimesheetLog;

namespace Timesheet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TimeSheetLogController : BaseController
    {
        private readonly ITimesheetAttendanceBLL _itimesheetAttendanceBLL;
        IHttpContextAccessor _httpContextAccessor;

        public TimeSheetLogController(ITimesheetAttendanceBLL itimesheetAttendanceBLL, IHttpContextAccessor httpContextAccessor) :base(httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            _itimesheetAttendanceBLL = itimesheetAttendanceBLL;
        }
        [HttpPost("PunchIn")]
        public async Task<IActionResult> PunchIn(PunchInDto dto)
        {
            var response = await _itimesheetAttendanceBLL.SubmitPunchInTime(UserId ?? 0, DateTime.Parse(dto.PunchIn));

            return Ok(response);
        }
       
        [HttpPost("PunchOut")]
        public async Task<IActionResult> PunchOut(PunchOutDto dto)
        {
            var response = await _itimesheetAttendanceBLL.SubmitPunchOutTime(UserId ?? 0, DateTime.Parse(dto.PunchOut)
);

            return Ok(response);
        }
        [HttpGet("GetTodayAttendance")]
        public async Task<IActionResult> GetTodayAttendance()
        {
            var response = await _itimesheetAttendanceBLL.GetTodayAttendance(UserId ?? 0);

            return Ok(response);
        }
        [HttpGet("GetAllAttendance")]
        public async Task<IActionResult> GetAllAttendance( int page = 0, int size = 20)
        {
            var response = await _itimesheetAttendanceBLL.GetAllAttendance(UserId??0,page,size);

            return Ok(response);
        }


    }
}
