// PhoneStore.API/Controllers/DashboardController.cs
using Microsoft.AspNetCore.Mvc;
using BusinessObject.ViewModels;
using Service.IServices;
using System.Threading.Tasks;

namespace PhoneStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(int? top = 5)
        {
            var data = await _dashboardService.GetDashboardData(top);
            return Ok(data);
        }

        [HttpGet("GetProfit")]
        public async Task<IActionResult> GetProfit(string startDate, string endDate)
        {
            DateTime? start = string.IsNullOrEmpty(startDate) ? (DateTime?)null : DateTime.Parse(startDate);
            DateTime? end = string.IsNullOrEmpty(endDate) ? (DateTime?)null : DateTime.Parse(endDate);

            decimal profit = await _dashboardService.GetProfit(start, end);
            return Ok(new { success = true, profit });
        }
    }
}