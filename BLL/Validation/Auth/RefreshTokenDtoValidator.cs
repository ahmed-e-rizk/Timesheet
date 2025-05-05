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
    public class RefreshTokenDtoValidator : DtoValidationAbstractBase<RefreshTokenDto>
    {
        public RefreshTokenDtoValidator()
        {

            RuleFor(x => x.RefreshToken)
                .NotNull().WithErrorCode(MessageCodes.Required)
                .NotEmpty().WithErrorCode(MessageCodes.Required);
        }
    }
}
