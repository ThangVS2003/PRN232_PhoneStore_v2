using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebMVC.Models;

namespace WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("products");
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<ProductDetailViewModel>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"products/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDetailViewModel>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(product);
        }
    }
}


