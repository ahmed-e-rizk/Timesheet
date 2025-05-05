using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Helper
{
    public class AuthSetting
    {
        public Jwt Jwt { get; set; } = new();
    }

    public class Jwt
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public TimeSpan TokenExpiryTime { get; set; }

        public RefreshTokenSetting RefreshToken { get; set; } = new();

    }
    public class RefreshTokenSetting
    {
        public int TokenLength { get; set; }

        public int RefreshTokenExpiryInMonths { get; set; }
    }
}
