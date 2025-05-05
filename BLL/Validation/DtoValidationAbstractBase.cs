using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Timesheet.BLL.Validation
{
    public class DtoValidationAbstractBase<T> : AbstractValidator<T> where T : class
    {
    }
}
