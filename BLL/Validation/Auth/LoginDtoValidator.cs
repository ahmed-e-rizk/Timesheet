using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Auth;
using DTO.Eunms;
using FluentValidation;
using Helper;

namespace Timesheet.BLL.Validation.Auth
{
    public class LoginDtoValidator : DtoValidationAbstractBase<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotNull().WithErrorCode(MessageCodes.Required)
                .NotEmpty().WithErrorCode(MessageCodes.Required)
                .EmailAddress().WithErrorCode(MessageCodes.InvalidEmail);

            RuleFor(x => x.Password)
                .NotNull().WithErrorCode(MessageCodes.Required)
                .NotEmpty().WithErrorCode(MessageCodes.Required);
        }
    }
}
