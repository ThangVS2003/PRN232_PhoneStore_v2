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
    [Route("Vouchers")]
    public class VouchersController : Controller
    {
        private readonly HttpClient _httpClient;

        public VouchersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("vouchers");
                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var content = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<VoucherViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View("~/Views/Staff/Vouchers/Index.cshtml", products);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string code)
        {
            try
            {
                HttpResponseMessage response;

                if (string.IsNullOrWhiteSpace(code))
                {
                    // Nếu không nhập code, gọi lại API lấy tất cả
                    response = await _httpClient.GetAsync("vouchers");
                }
                else
                {
                    response = await _httpClient.GetAsync($"vouchers/search?code={code}");
                }

                var vouchers = new List<VoucherViewModel>();

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    vouchers = JsonSerializer.Deserialize<List<VoucherViewModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    vouchers = new List<VoucherViewModel>();
                }
                else
                {
                    return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }

                ViewBag.SelectedCode = code;

                return View("~/Views/Staff/Vouchers/Index.cshtml", vouchers);
            }
            catch
            {
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] VoucherViewModel dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("vouchers", content);

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi tạo voucher: " + ex.Message);
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VoucherViewModel dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest("ID không khớp.");

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"vouchers/{id}", content);

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi cập nhật voucher: " + ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"vouchers/{id}");

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi xóa voucher: " + ex.Message);
            }
        }
    }
}
