using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DTO.Auth;
using DTO.Eunms;
using FluentValidation;
using Helper;

namespace Timesheet.BLL.Validation.Auth
{
    public class RegisterDtoValidator : DtoValidationAbstractBase<RegisterDto>
    {
        public RegisterDtoValidator()
        {

            RuleFor(e => e.Name).NotEmpty().WithErrorCode(MessageCodes.NameNotEmpty);
            RuleFor(x => x.Email)
               .NotNull().NotEmpty().EmailAddress().WithErrorCode(MessageCodes.InvalidEmail);
            RuleFor(x => x.Password)
        .NotEmpty().WithErrorCode(MessageCodes.NotMatchPass)
        .NotNull().WithErrorCode(MessageCodes.NotMatchPass)
        .MinimumLength(5).WithErrorCode(MessageCodes.NotMatchPass)
        .MaximumLength(20).WithErrorCode(MessageCodes.NotMatchPass)
        .Must(password => HasLetterAndSpecialCharacter(password)).WithErrorCode(MessageCodes.NotMatchPass);

        }
        private bool HasLetterAndSpecialCharacter(string password)
        {
            return Regex.IsMatch(password, @"(?=.*[A-Za-z])(?=.*\W)");
        }
    }
}
