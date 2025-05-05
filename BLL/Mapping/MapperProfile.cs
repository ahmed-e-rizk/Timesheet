using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DTO.Auth;
using Timesheet.Core.Entites;
using Timesheet.DTO.TimesheetLog;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Timesheet.BLL.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(e => e.Email, opt => opt.MapFrom(e => e.Email))
                .ForMember(e => e.Name, opt => opt.MapFrom(e => e.Name))
                .ForMember(e => e.Password, opt => opt.MapFrom(e => e.Password))
                .ForMember(e => e.MobileNumber, opt => opt.MapFrom(e => e.MobileNumber)).ReverseMap();
            CreateMap<AttendanceLogsDto, AttendanceLogs>().ReverseMap();
        }
    }
}
