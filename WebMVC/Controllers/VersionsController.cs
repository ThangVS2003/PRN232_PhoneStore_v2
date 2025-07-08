using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    [Authorize(Roles = "2")]
    [Route("Versions")]
    public class VersionsController : Controller
    {
        private readonly HttpClient _httpClient;

        public VersionsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("versions");
                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var content = await response.Content.ReadAsStringAsync();
                var versions = JsonSerializer.Deserialize<List<VersionViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View("~/Views/Staff/Versions/Index.cshtml", versions);
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
                    response = await _httpClient.GetAsync("versions");
                }
                else
                {
                    response = await _httpClient.GetAsync($"versions/search?name={name}");
                }

                var versions = new List<VersionViewModel>();

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    versions = JsonSerializer.Deserialize<List<VersionViewModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    versions = new List<VersionViewModel>();
                }
                else
                {
                    return View("Error");
                }

                ViewBag.SelectedName = name;

                return View("~/Views/Staff/Versions/Index.cshtml", versions);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] VersionViewModel dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("versions", content);
                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi tạo version: " + ex.Message);
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VersionViewModel dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest("ID không khớp.");

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"versions/{id}", content);
                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi cập nhật version: " + ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"versions/{id}");
                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi xóa version: " + ex.Message);
            }
        }
    }
}
