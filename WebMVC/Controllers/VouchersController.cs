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
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                int pageSize = 5;
                var response = await _httpClient.GetAsync($"vouchers?isPaging=true&page={page}&pageSize={pageSize}");
                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);

                var vouchersJson = json.RootElement.GetProperty("data").GetRawText();
                var vouchers = JsonSerializer.Deserialize<List<VoucherViewModel>>(vouchersJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                int totalItems = json.RootElement.GetProperty("totalItems").GetInt32();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View("~/Views/Staff/Vouchers/Index.cshtml", vouchers);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string code, int page = 1)
        {
            try
            {
                int pageSize = 5;
                HttpResponseMessage response;

                if (string.IsNullOrWhiteSpace(code))
                {
                    // Lấy tất cả với phân trang
                    response = await _httpClient.GetAsync($"vouchers?isPaging=true&page={page}&pageSize={pageSize}");
                }
                else
                {
                    response = await _httpClient.GetAsync($"vouchers/search?code={code}&page={page}&pageSize={pageSize}");
                }

                var vouchers = new List<VoucherViewModel>();
                int totalItems = 0;

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(content);

                    var vouchersJson = json.RootElement.GetProperty("data").GetRawText();
                    vouchers = JsonSerializer.Deserialize<List<VoucherViewModel>>(vouchersJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    totalItems = json.RootElement.GetProperty("totalItems").GetInt32();
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    vouchers = new List<VoucherViewModel>();
                }
                else
                {
                    return View("Error");
                }

                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                ViewBag.SelectedCode = code;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.IsSearch = true;

                return View("~/Views/Staff/Vouchers/Index.cshtml", vouchers);
            }
            catch
            {
                return View("Error");
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
