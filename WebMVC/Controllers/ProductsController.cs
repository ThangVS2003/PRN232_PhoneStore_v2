using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;
using WebMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebMVC.Controllers
{
    [Authorize(Roles = "2")]
    [Route("Products")]
    public class ProductsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var response = await _httpClient.GetAsync($"products?includeDeleted=true&page={page}&pageSize=5");
                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(content);

                var productsJson = result.RootElement.GetProperty("data").GetRawText();
                var products = JsonSerializer.Deserialize<List<ProductDetailViewModel>>(productsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var totalItems = result.RootElement.GetProperty("totalItems").GetInt32();
                var totalPages = (int)Math.Ceiling(totalItems / 5.0);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                var brandList = await GetBrandSelectList();
                ViewBag.Brands = brandList;

                return View("~/Views/Staff/Products/Index.cshtml", products);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string name, int brandId, int page = 1)
        {
            try
            {
                var url = $"products/by-name-and-brand?name={name}&brandId={brandId}&page={page}&pageSize=5";
                var response = await _httpClient.GetAsync(url);

                List<ProductDetailViewModel> products = new List<ProductDetailViewModel>();
                int totalItems = 0;

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonDocument.Parse(content);

                    var productsJson = result.RootElement.GetProperty("data").GetRawText();
                    products = JsonSerializer.Deserialize<List<ProductDetailViewModel>>(productsJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    totalItems = result.RootElement.GetProperty("totalItems").GetInt32();
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    products = new List<ProductDetailViewModel>();
                }
                else
                {
                    return View("Error");
                }

                var totalPages = (int)Math.Ceiling(totalItems / 5.0);

                var brandList = await GetBrandSelectList();
                ViewBag.Brands = brandList;
                ViewBag.SelectedName = name;
                ViewBag.SelectedBrandId = brandId;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.IsSearch = true; // thêm flag để biết đang ở search

                return View("~/Views/Staff/Products/Index.cshtml", products);
            }
            catch
            {
                return View("Error");
            }
        }

        private async Task<List<SelectListItem>> GetBrandSelectList()
        {
            try
            {
                var response = await _httpClient.GetAsync("brands");
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

                list.Insert(0, new SelectListItem { Value = "0", Text = "All" });

                return list;
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ProductCreateViewModel dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("products", content);

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi tạo sản phẩm: " + ex.Message);
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateViewModel dto)
        {
            try
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
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi cập nhật sản phẩm: " + ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"products/{id}");

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi xóa sản phẩm: " + ex.Message);
            }
        }

        [HttpPut("Restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"products/restore/{id}", null);

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi khôi phục sản phẩm: " + ex.Message);
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var productResponse = await _httpClient.GetAsync($"products/{id}?includeDeleted=true");
                if (!productResponse.IsSuccessStatusCode)
                    return NotFound();

                var productContent = await productResponse.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductDetailViewModel>(productContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Lấy Feedback 
                try
                {
                    var feedbackResponse = await _httpClient.GetAsync($"feedbackproducts/product/{id}");
                    if (feedbackResponse.IsSuccessStatusCode)
                    {
                        var feedbackContent = await feedbackResponse.Content.ReadAsStringAsync();
                        var feedbacks = JsonSerializer.Deserialize<List<ProductFeedbackViewModel>>(feedbackContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        product.Feedbacks = feedbacks ?? new List<ProductFeedbackViewModel>();
                    }
                    else
                    {
                        product.Feedbacks = new List<ProductFeedbackViewModel>();
                    }
                }
                catch
                {
                    product.Feedbacks = new List<ProductFeedbackViewModel>();
                }

                ViewBag.Colors = await GetColorSelectList();
                ViewBag.Versions = await GetVersionSelectList();
                return View("~/Views/Staff/Products/Details.cshtml", product);
            }
            catch
            {
                return View("Error");
            }
        }

        private async Task<List<SelectListItem>> GetColorSelectList()
        {
            try
            {
                var response = await _httpClient.GetAsync("colors");
                var content = await response.Content.ReadAsStringAsync();
                var colors = JsonSerializer.Deserialize<List<ColorViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return colors.Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                }).ToList();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }

        private async Task<List<SelectListItem>> GetVersionSelectList()
        {
            try
            {
                var response = await _httpClient.GetAsync("versions");
                var content = await response.Content.ReadAsStringAsync();
                var versions = JsonSerializer.Deserialize<List<VersionViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return versions.Select(v => new SelectListItem
                {
                    Value = v.Name,
                    Text = v.Name
                }).ToList();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }
    }
}

