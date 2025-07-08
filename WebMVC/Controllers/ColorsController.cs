using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    [Authorize(Roles = "2")]
    [Route("Colors")]
    public class ColorsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ColorsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("colors");
                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var content = await response.Content.ReadAsStringAsync();
                var colors = JsonSerializer.Deserialize<List<ColorViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View("~/Views/Staff/Colors/Index.cshtml", colors);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string name)
        {
            try
            {
                HttpResponseMessage response;

                if (string.IsNullOrWhiteSpace(name))
                {
                    response = await _httpClient.GetAsync("colors");
                }
                else
                {
                    response = await _httpClient.GetAsync($"colors/search?name={name}");
                }

                var colors = new List<ColorViewModel>();

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    colors = JsonSerializer.Deserialize<List<ColorViewModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    colors = new List<ColorViewModel>();
                }
                else
                {
                    return View("Error");
                }

                ViewBag.SelectedName = name;

                return View("~/Views/Staff/Colors/Index.cshtml", colors);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ColorViewModel dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("colors", content);
                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi tạo color: " + ex.Message);
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ColorViewModel dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest("ID không khớp.");

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"colors/{id}", content);
                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi cập nhật color: " + ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"colors/{id}");

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi xóa color: " + ex.Message);
            }
        }
    }
}
