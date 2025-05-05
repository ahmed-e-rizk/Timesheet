using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Response
{
   
    public class TErrorField
    {
        public string FieldName { get; set; } = string.Empty;
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
