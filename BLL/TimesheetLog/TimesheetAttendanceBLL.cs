using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.BaseResponse;
using DTO.Auth;
using DTO.Eunms;
using Helper;
using Microsoft.EntityFrameworkCore;
using Repositroy;
using Timesheet.Core.Entites;
using Timesheet.DTO;
using Timesheet.DTO.TimesheetLog;
using Timesheet.Helper;
using Timesheet.Repositroy.Infrastructure;

namespace Timesheet.BLL.TimesheetLog
{
    public class TimesheetAttendanceBLL : ITimesheetAttendanceBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private readonly IRepository<AttendanceLogs> _attendanceLogsRepository;
        public TimesheetAttendanceBLL(IUnitOfWork unitOfWork, IMapper mapper, IRepository<AttendanceLogs> attendanceLogsrepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attendanceLogsRepository = attendanceLogsrepository;


        }
        public async Task<IResponse<PagedResultDto<AttendanceLogsDto>>> GetAllAttendance(int userId, int page, int size)
        {
            var response = new Response<PagedResultDto<AttendanceLogsDto>>();
            try
            {
               var count  = _attendanceLogsRepository.Where(e => e.UserId == userId).Count();

                List<AttendanceLogs> Attendances = _attendanceLogsRepository.Where(e => e.UserId == userId, page * size, size).ToList();
                var result = _mapper.Map<List<AttendanceLogsDto>>(Attendances);
                var Data = new PagedResultDto<AttendanceLogsDto> {

                    Items = result,
                    TotalCount = count,
                    PageNumber= page,
                    PageSize = size

                };

                var x = response.CreateResponse(Data);
                return response.CreateResponse(Data);

            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }

        }

        public async Task<IResponse<AttendanceLogsDto>> GetTodayAttendance(int userId)
        {
            var response = new Response<AttendanceLogsDto>();
            try
            {
                var today = DateTime.UtcNow.Date;
                var log = await _attendanceLogsRepository.GetAsync(e => e.UserId == userId && e.Date == DateOnly.FromDateTime(today));
                var result = _mapper.Map<AttendanceLogsDto>(log);
                response.CreateResponse(result);
                
            }
            catch(Exception ex)
            {
                response.CreateResponse(ex);

            }
            return response;
        }

        public  async Task<IResponse<bool>> SubmitPunchInTime(int userId, DateTime? PunchIn = null)
        {

            var response = new Response<bool>();

            try
            {
                var punchInTime = PunchIn ?? DateTime.UtcNow;
                var punchInDate = DateOnly.FromDateTime(punchInTime);

                var existingLog = await _attendanceLogsRepository.GetAsync(
                    l => l.UserId == userId && l.Date == punchInDate);

                if (existingLog != null)
                {
                    if (PunchIn == null)
                        return response.CreateResponse(MessageCodes.PunchIn); 

                    existingLog.LoginTime = punchInTime;
                    await _unitOfWork.CommitAsync();
                    return response.CreateResponse(true);
                }

                await _attendanceLogsRepository.AddAsync(new AttendanceLogs
                {
                    UserId = userId,
                    Date = punchInDate,
                    LoginTime = punchInTime
                });

                await _unitOfWork.CommitAsync();
                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }

        }
        public  async Task<IResponse<bool>> SubmitPunchOutTime(int userId, DateTime? PunchOut = null)
        {
            var response = new Response<bool>();

            try
            {
                var punchOutTime = PunchOut ?? DateTime.UtcNow;
                var punchOutDate = DateOnly.FromDateTime(punchOutTime);

                var existingLog = await _attendanceLogsRepository.GetAsync(
                    l => l.UserId == userId && l.Date == punchOutDate);

                if (existingLog != null)
                {
                    if (PunchOut == null)
                        return response.CreateResponse(MessageCodes.PunchOut);

                    existingLog.LogoutTime = punchOutTime;
                    await _unitOfWork.CommitAsync();
                    return response.CreateResponse(true);
                }

                await _attendanceLogsRepository.AddAsync(new AttendanceLogs
                {
                    UserId = userId,
                    Date = punchOutDate,
                    LogoutTime = punchOutTime
                });

                await _unitOfWork.CommitAsync();
                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }

        }


    }
}
