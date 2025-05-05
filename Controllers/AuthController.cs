using BLL.Auth;
using DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Timesheet.DTO.Auth;

namespace Timesheet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthBLL _authBLL;
        IHttpContextAccessor _httpContextAccessor;

        public AuthController(IAuthBLL authBLL, IHttpContextAccessor httpContextAccessor) :base(httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _authBLL = authBLL;
        }
        [AllowAnonymous]
        [HttpPost("Register")]

        public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
        {
            var response = await _authBLL.RegisterAsync(registerDto);

            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            var response = await _authBLL.LoginAsync(loginDto);

            return Ok(response);
        }
       
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var response = await _authBLL.RefreshTokenAsync(refreshTokenDto);

            if (response.IsSuccess)
                return Ok(response);
            else
                return Unauthorized();
        }
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutAsync(LogoutDto logoutDto)
        {
            var response = await _authBLL.LogoutAsync(logoutDto);

            return Ok(response);
        }

    }
}
