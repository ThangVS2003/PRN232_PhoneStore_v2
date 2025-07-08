using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;
using WebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace WebMVC.Controllers;
[Authorize(Roles = "2")]
[Route("Brands")]
public class BrandsController : Controller
{
    private readonly HttpClient _httpClient;

    public BrandsController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
        _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _httpClient.GetAsync("brands");
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var content = await response.Content.ReadAsStringAsync();
            var brands = JsonSerializer.Deserialize<List<BrandViewModel>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View("~/Views/Staff/Brands/Index.cshtml", brands);
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
                response = await _httpClient.GetAsync("brands");
            }
            else
            {
                response = await _httpClient.GetAsync($"brands/search?name={name}");
            }

            var brands = new List<BrandViewModel>();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                brands = JsonSerializer.Deserialize<List<BrandViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                brands = new List<BrandViewModel>();
            }
            else
            {
                return View("Error");
            }

            ViewBag.SelectedName = name;

            return View("~/Views/Staff/Brands/Index.cshtml", brands);
        }
        catch
        {
            return View("Error");
        }
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] BrandViewModel dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("brands", content);
            if (response.IsSuccessStatusCode)
                return Ok();

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }
        catch (Exception ex)
        {
            return BadRequest("Lỗi khi tạo brand: " + ex.Message);
        }
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BrandViewModel dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest("ID không khớp.");

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"brands/{id}", content);
            if (response.IsSuccessStatusCode)
                return Ok();

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }
        catch (Exception ex)
        {
            return BadRequest("Lỗi khi cập nhật brand: " + ex.Message);
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"brands/{id}");

            if (response.IsSuccessStatusCode)
                return Ok();

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }
        catch (Exception ex)
        {
            return BadRequest("Lỗi khi xóa brand: " + ex.Message);
        }
    }
}
