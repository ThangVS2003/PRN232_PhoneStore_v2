using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;
using WebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace WebMVC.Controllers
{
    [Authorize(Roles = "2")]
    [Route("FeedbackProductsStaff")]
    public class FeedbackProductsStaffController : Controller
    {
        private readonly HttpClient _httpClient;

        public FeedbackProductsStaffController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"feedbackproducts/{id}");

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi xóa feedback: " + ex.Message);
            }
        }
    }
}
