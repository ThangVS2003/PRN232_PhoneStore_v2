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
    [Route("Serials")]
    public class SerialsController : Controller
    {
        private readonly HttpClient _httpClient;

        public SerialsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
        }

        // Mặc định truy cập Serial theo VariantId (nút từ Details)
        [HttpGet("Serial/{variantId}")]
        public async Task<IActionResult> Serial(int variantId, int productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"serials/variant/{variantId}");

                var serials = new List<SerialViewModel>();
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    serials = JsonSerializer.Deserialize<List<SerialViewModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                ViewBag.ProductId = productId;
                ViewBag.VariantId = variantId;
                ViewBag.SelectedSerialNumber = string.Empty;

                return View("~/Views/Staff/Products/Serial.cshtml", serials);
            }
            catch
            {
                return View("Error");
            }
        }

        // Search theo SerialNumber + ProductVariantId
        [HttpGet("Search")]
        public async Task<IActionResult> Search(string serialNumber, int variantId, int productId)
        {
            try
            {
                HttpResponseMessage response;

                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    // Không nhập thì lấy theo variantId
                    response = await _httpClient.GetAsync($"serials/variant/{variantId}");
                }
                else
                {
                    // Có nhập thì search theo cả serial + variantId
                    response = await _httpClient.GetAsync($"serials/search-by-productVariantId?serialNumber={serialNumber}&productVariantId={variantId}");
                }

                var serials = new List<SerialViewModel>();

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    serials = JsonSerializer.Deserialize<List<SerialViewModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    serials = new List<SerialViewModel>();
                }
                else
                {
                    return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }

                ViewBag.ProductId = productId;
                ViewBag.VariantId = variantId;
                ViewBag.SelectedSerialNumber = serialNumber;

                return View("~/Views/Staff/Products/Serial.cshtml", serials);
            }
            catch
            {
                return View("Error");
            }
        }
    }
}
