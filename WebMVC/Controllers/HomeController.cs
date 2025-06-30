using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BusinessObject.Models;

namespace PhoneStoreMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
        }

        public async Task<IActionResult> Index(int? brandId, decimal? minPrice, decimal? maxPrice, string? name, int page = 1, int pageSize = 6)
        {
            try
            {
                // Lấy danh sách thương hiệu
                var brandsResponse = await _httpClient.GetFromJsonAsync<List<Brand>>("api/Brands");
                ViewBag.Brands = brandsResponse ?? new List<Brand>();
                Console.WriteLine($"Brands count: {brandsResponse?.Count ?? 0}");

                // Đếm số sản phẩm theo thương hiệu
                var brandProductCounts = new Dictionary<int, int>();
                foreach (var brand in ViewBag.Brands)
                {
                    try
                    {
                        Console.WriteLine($"Calling API: api/Products/by-brand/{brand.Id}");
                        var productsByBrand = await _httpClient.GetFromJsonAsync<List<Product>>($"api/Products/by-brand/{brand.Id}");
                        Console.WriteLine($"Products for BrandId {brand.Id}: {productsByBrand?.Count ?? 0}");
                        brandProductCounts[brand.Id] = productsByBrand?.Count ?? 0;
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Error calling API for BrandId {brand.Id}: {ex.Message}");
                        brandProductCounts[brand.Id] = 0;
                    }
                }
                ViewBag.BrandProductCounts = brandProductCounts;

                // Lấy tất cả sản phẩm
                List<Product> allProducts;
                try
                {
                    Console.WriteLine("Calling API: api/Products/all");
                    allProducts = await _httpClient.GetFromJsonAsync<List<Product>>("api/Products/all");
                    Console.WriteLine($"Total products from api/Products: {allProducts?.Count ?? 0}");

                    // Lọc thủ công theo brandId và name
                    if (brandId.HasValue)
                        allProducts = allProducts.Where(p => p.BrandId == brandId.Value).ToList();
                    if (!string.IsNullOrEmpty(name))
                        allProducts = allProducts.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error calling api/Products: {ex.Message}");
                    allProducts = new List<Product>();
                }

                // Phân trang
                int totalItems = allProducts.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var paginatedProducts = allProducts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.Products = paginatedProducts;
                ViewBag.SelectedBrandId = brandId;
                ViewBag.MinPrice = minPrice ?? 0;
                ViewBag.MaxPrice = maxPrice ?? 1000000;
                ViewBag.Name = name;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Index: {ex.Message}");
                ViewBag.Brands = new List<Brand>();
                ViewBag.Products = new List<Product>();
                ViewBag.BrandProductCounts = new Dictionary<int, int>();
                return View();
            }
        }
    }
}