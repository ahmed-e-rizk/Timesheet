
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions;
using System.Net.Http;

using System.Net.Http.Json;
using DTO.Auth;
using BLL.BaseResponse;

namespace Timesheet.BLL.Middleware
{

    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;

        public TokenMiddleware(RequestDelegate next, System.Net.Http.IHttpClientFactory httpClient)
        {
            _next = next;
            _httpClient = httpClient.CreateClient("TimeSheet");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = context.Request.Cookies["AccessToken"];
            var refreshToken = context.Request.Cookies["RefreshToken"];

            if (accessToken == null && refreshToken != null)
            {
                // Refresh the token
                var x = JsonContent.Create(new TokenResultDto { RefreshToken = refreshToken });

                var response = await _httpClient.PostAsync("Auth/RefreshToken", x);
                var res = await response.Content.ReadFromJsonAsync<Response<TokenResultDto>>();

                // Update the cookie with the new access token
                context.Response.Cookies.Append("AccessToken", res.Data.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMinutes(25)
                });
                context.Response.Cookies.Append("RefreshToken", res.Data.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(30)
                });

               
            }
           


            await _next(context);
        }
    }
}
