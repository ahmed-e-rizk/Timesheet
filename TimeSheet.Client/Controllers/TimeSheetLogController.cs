using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Timesheet.DTO.TimesheetLog;
using Timesheet.DTO;
using System.Security.Policy;
using BLL.BaseResponse;

namespace TimeSheet.Client.Controllers
{
   
    public class TimeSheetLogController : Controller
    {
        private readonly HttpClient _httpClient;
        public TimeSheetLogController(IHttpClientFactory httpClientFactory):base()
        {
            _httpClient = httpClientFactory.CreateClient("TimeSheet");

        }
        [HttpGet]
        public async Task<JsonResult> GetPagedAttendanceLogs(int page, int size)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request?.Cookies["AccessToken"]);

            var response = await _httpClient.GetFromJsonAsync<Response<PagedResultDto<AttendanceLogsDto>>>(
                $"TimeSheetLog/GetAllAttendance?page={page}&size={size}");

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> PunchIn([FromBody] PunchInDto dto)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request?.Cookies["AccessToken"]);

            var response = await _httpClient.PostAsJsonAsync("TimeSheetLog/PunchIn", dto);
            if (response.IsSuccessStatusCode)
            {
                
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetTodayAttendance()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request?.Cookies["AccessToken"]);

            var response = await _httpClient.GetFromJsonAsync<AttendanceLogsDto>("TimeSheetLog/GetTodayAttendance");
           
                
                return Ok(response);
           
        }

        [HttpPost]
        public async Task<IActionResult> PunchOut([FromBody] PunchOutDto dto)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request?.Cookies["AccessToken"]);

            var response = await _httpClient.PostAsJsonAsync("TimeSheetLog/PunchOut", dto);
            if (response.IsSuccessStatusCode)
            {

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
