using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Auth
{
    public class JwtTokenDto
    {
        public string Jti { get; set; }

        public string Token { get; set; }
    }
}
