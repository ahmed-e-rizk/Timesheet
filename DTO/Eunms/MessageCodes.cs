using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Eunms
{
    public enum MessageCodes
    {
        [Description("Failed : Email Already Exists")]
        AlreadyExists = 1000,
        [Description("Failed : Name isn`t Empty")]
        NameNotEmpty = 1001,
        [Description("Failed :  dosn`t Email")]

        InvalidEmail = 1002,
        [Description("Failed :  Password must be between 5 and 20 characters long, and must contain at least one letter and one special character. ")]

        NotMatchPass = 1003,
        [Description("Failed :  Required")]

        Required = 1004,
        [Description("Failed :Invalid login credentials")]

        InvalidLoginCredentials = 1005,
        [Description("Failed :Invalid Token")]

        InvalidToken = 1006,
        [Description("Failed :Not Found")]

        NotFound = 1007,
        [Description("Failed :Tokens DoNot Match")]

        TokensDoNotMatch = 1008,
          [Description("Already Punch In Today")]

        PunchIn = 1009 ,
        [Description("Already Punch Out Today")]

        PunchOut = 1009
    }
}
