using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text;
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

            var brandList = await GetBrandSelectList();
            ViewBag.Brands = brandList;
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string name, int brandId)
        {
            var url = $"products/by-name-and-brand?name={name}&brandId={brandId}";
            var response = await _httpClient.GetAsync(url);

            List<ProductDetailViewModel> products = new List<ProductDetailViewModel>();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                products = JsonSerializer.Deserialize<List<ProductDetailViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // Trường hợp không có sản phẩm phù hợp, trả về danh sách rỗng
                products = new List<ProductDetailViewModel>();
            }
            else
            {
                return View("Error");
            }

            var brandList = await GetBrandSelectList();
            ViewBag.Brands = brandList;
            ViewBag.SelectedName = name;
            ViewBag.SelectedBrandId = brandId;

            return View("Index", products);
        }

        private async Task<List<SelectListItem>> GetBrandSelectList()
        {
            var response = await _httpClient.GetAsync("brands"); // endpoint trả về danh sách brand
            var content = await response.Content.ReadAsStringAsync();
            var brands = JsonSerializer.Deserialize<List<BrandViewModel>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var list = brands.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            // Thêm mục All
            list.Insert(0, new SelectListItem { Value = "0", Text = "All" });

            return list;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateViewModel dto)
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("products", content);

            if (response.IsSuccessStatusCode)
                return Ok();

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }

        [HttpPut]
        [Route("Admin/Products/Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateViewModel dto)
        {
            if (id != dto.Id)
                return BadRequest("ID không khớp.");

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"products/{id}", content);

            if (response.IsSuccessStatusCode)
                return NoContent();

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }

        [HttpDelete("Admin/Products/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"products/{id}");

            if (response.IsSuccessStatusCode)
                return Ok();

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }

        public async Task<IActionResult> Details(int id)
        {
            // Lấy thông tin sản phẩm theo id
            var response = await _httpClient.GetAsync($"products/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDetailViewModel>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Trả về view với full thông tin sản phẩm + danh sách biến thể
            return View(product);
        }
    }
}


