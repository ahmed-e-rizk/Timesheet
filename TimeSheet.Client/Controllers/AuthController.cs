using BLL.BaseResponse;
using DTO.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Timesheet.DTO.Auth;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace TimeSheet.Client.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TimeSheet");
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromForm] RegisterDto input)
        {
            var x = JsonContent.Create(input);
            var result = await _httpClient.PostAsync("Auth/Register", x);
            if (result.IsSuccessStatusCode)
            {
                var resp = await result.Content.ReadFromJsonAsync<Response<bool>>();
                if (!resp.IsSuccess)
                {
                    ViewData["Error"] = resp.Errors;
                    return View();
                }
            }
            else
            {
                return View();
            }

            return Redirect("Login");
        }
        [HttpGet]

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromForm] LoginDto inputDto)
        {
            var x = JsonContent.Create(inputDto);

            var result = await _httpClient.PostAsync("Auth/Login", x);
            if (result.IsSuccessStatusCode)
            {
                var resp = await result.Content.ReadFromJsonAsync<Response<TokenResultDto>>();
                if (!resp.IsSuccess)
                {
                    ViewData["Error"] = resp.Errors;
                    return View();
                }

                Response.Cookies.Append("AccessToken", resp.Data.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMinutes(25)
                });
                Response.Cookies.Append("RefreshToken", resp.Data.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(30)
                });
                TempData["login"] = true;
                return Redirect("/Home/Index");
            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            LogoutDto logoutDto= new LogoutDto
            {
                RefreshToken = Request?.Cookies["RefreshToken"]
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request?.Cookies["AccessToken"]);

            var result = await _httpClient.PostAsJsonAsync("Auth/Logout", logoutDto);



            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");
            return RedirectToAction("login", "Auth");
        }
    }
}
