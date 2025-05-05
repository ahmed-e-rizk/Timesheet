using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.BaseResponse;
using DTO.Auth;
using Microsoft.IdentityModel.Tokens;
using Timesheet.DTO.Auth;

namespace BLL.Auth
{
    public interface IAuthBLL
    {
        Task<IResponse<bool>> RegisterAsync(RegisterDto registerDto);
        Task<IResponse<TokenResultDto>> LoginAsync(LoginDto loginDto);
        Task<IResponse<TokenResultDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<IResponse<bool>> LogoutAsync(LogoutDto logoutDto);
    }
}
