using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PhoneStoreMVC.Controllers
{
    [Authorize(Roles = "2")] // Chỉ cho phép vai trò Staff (role = 2) truy cập
    public class StaffController : Controller
    {
        private readonly HttpClient _httpClient;

        public StaffController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["UserName"] = User?.FindFirst(ClaimTypes.Name)?.Value ?? "Staff";
            return View("~/Views/Staff/StaffDashboard.cshtml");
        }
    }
}