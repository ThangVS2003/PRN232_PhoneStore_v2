using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebMVC.Models;

                using System.Net.Http;
                using System.Text.Json;
                using System.Threading.Tasks;
                using WebMVC.Models;

                namespace WebMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
        }

        public async Task<IActionResult>
            Index()
        {
            var response = await _httpClient.GetAsync("products/all");
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
            var response = await _httpClient.GetAsync($"products/all/{id}");
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