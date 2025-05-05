using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Timesheet.BLL.TimesheetLog;

namespace Timesheet.Controllers
{

   
    public class BaseController : ControllerBase
    {
        protected string Token { get; private set; }
        public int? UserId { get; private set; }

        public BaseController(IHttpContextAccessor  httpContextAccessor)
        {
            var authHeader = httpContextAccessor?.HttpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                Token = authHeader.Substring("Bearer ".Length);

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(Token);

                // You can change "sub" to whatever your claim name for user id is
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "Id");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int uid))
                {
                    UserId = uid;
                }
            }
        }
        
            // Extract token
           
        
    }

}
