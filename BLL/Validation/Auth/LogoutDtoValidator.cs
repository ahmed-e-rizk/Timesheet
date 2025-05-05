using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Eunms;
using FluentValidation;
using Helper;
using Timesheet.DTO.Auth;

namespace Timesheet.BLL.Validation.Auth
{
    public class LogoutDtoValidator : DtoValidationAbstractBase<LogoutDto>
    {
        public LogoutDtoValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotNull().WithErrorCode(MessageCodes.Required)
                .NotEmpty().WithErrorCode(MessageCodes.Required);
        }
    }
}
