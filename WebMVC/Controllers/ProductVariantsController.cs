﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;
using WebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace WebMVC.Controllers
{
    [Authorize(Roles = "2")]
    [Route("ProductVariants")]
    public class ProductVariantsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductVariantsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
        }

        [HttpGet("Serial/{variantId}")]
        public async Task<IActionResult> Serial(int variantId, int productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"serials/variant/{variantId}");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ProductId = productId;
                    ViewBag.VariantId = variantId;
                    return View("~/Views/Staff/Products/Serial.cshtml", new List<SerialViewModel>());
                }

                var content = await response.Content.ReadAsStringAsync();
                var serials = JsonSerializer.Deserialize<List<SerialViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ViewBag.ProductId = productId;
                ViewBag.VariantId = variantId;
                return View("~/Views/Staff/Products/Serial.cshtml", serials);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost("CreateVariant")]
        public async Task<IActionResult> Create([FromBody] ProductVariantCreateViewModel dto)
        {
            try
            {
                // Gọi API phụ để lấy ColorId từ tên
                var colorRes = await _httpClient.GetAsync($"colors");
                var versionRes = await _httpClient.GetAsync($"versions");

                if (!colorRes.IsSuccessStatusCode || !versionRes.IsSuccessStatusCode)
                    return BadRequest("Không thể lấy dữ liệu màu sắc hoặc phiên bản.");

                var colorsJson = await colorRes.Content.ReadAsStringAsync();
                var versionsJson = await versionRes.Content.ReadAsStringAsync();

                var colors = JsonSerializer.Deserialize<List<ColorViewModel>>(colorsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var versions = JsonSerializer.Deserialize<List<VersionViewModel>>(versionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var color = colors?.FirstOrDefault(c => c.Name == dto.Color);
                var version = versions?.FirstOrDefault(v => v.Name == dto.Version);

                if (color == null || version == null)
                    return BadRequest("Color hoặc Version không hợp lệ.");

                // Chuẩn bị dữ liệu gửi lên API chính
                var apiDto = new
                {
                    ProductId = dto.ProductId,
                    ColorId = color.Id,
                    VersionId = version.Id,
                    OriginalPrice = dto.OriginalPrice,
                    SellingPrice = dto.SellingPrice,
                    StockQuantity = dto.StockQuantity,
                    Image = dto.Image
                };

                var json = JsonSerializer.Serialize(apiDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("productvariants", content);

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi tạo biến thể sản phẩm: " + ex.Message);
            }
        }

        [HttpPut("UpdateVariant/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductVariantCreateViewModel dto)
        {
            try
            {
                // Lấy ColorId, VersionId
                var colorRes = await _httpClient.GetAsync($"colors");
                var versionRes = await _httpClient.GetAsync($"versions");

                if (!colorRes.IsSuccessStatusCode || !versionRes.IsSuccessStatusCode)
                    return BadRequest("Không thể lấy dữ liệu màu sắc hoặc phiên bản.");

                var colors = JsonSerializer.Deserialize<List<ColorViewModel>>(
                    await colorRes.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var versions = JsonSerializer.Deserialize<List<VersionViewModel>>(
                    await versionRes.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var color = colors?.FirstOrDefault(c => c.Name == dto.Color);
                var version = versions?.FirstOrDefault(v => v.Name == dto.Version);

                if (color == null || version == null)
                    return BadRequest("Color hoặc Version không hợp lệ.");

                var apiDto = new
                {
                    Id = id,
                    ProductId = dto.ProductId,
                    ColorId = color.Id,
                    VersionId = version.Id,
                    OriginalPrice = dto.OriginalPrice,
                    SellingPrice = dto.SellingPrice,
                    StockQuantity = dto.StockQuantity,
                    Image = dto.Image
                };

                var json = JsonSerializer.Serialize(apiDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"productvariants/{id}", content);

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi cập nhật biến thể sản phẩm: " + ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"productvariants/{id}");

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi xóa biến thể: " + ex.Message);
            }
        }

        [HttpPut("Restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"productvariants/restore/{id}", null);

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi khôi phục biến thể: " + ex.Message);
            }
        }


        //////////////////// TRang Variants riêng (bỏ qua)
        // GET: /ProductVariants?page=1
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var response = await _httpClient.GetAsync($"ProductVariants?page={page}&pageSize=5");
                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(content);

                var json = result.RootElement.GetProperty("data").GetRawText();
                var variants = JsonSerializer.Deserialize<List<ProductVariantViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                int totalItems = result.RootElement.GetProperty("totalItems").GetInt32();
                int totalPages = (int)Math.Ceiling(totalItems / 5.0);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.IsSearch = false;

                // Lấy danh sách Colors & Versions cho dropdown
                ViewBag.Colors = await GetColorSelectList();
                ViewBag.Versions = await GetVersionSelectList();

                return View("~/Views/Staff/ProductVariants/Index.cshtml", variants);
            }
            catch
            {
                return View("Error");
            }
        }

        // GET: /ProductVariants/Search?productName=...&color=...&version=...&page=1
        //[HttpGet("Search")]
        //public async Task<IActionResult> Search(string productName, string color, string version, int page = 1)
        //{
        //    try
        //    {
        //        var url = $"ProductVariants/search?productName={productName}&color={color}&version={version}&page={page}&pageSize=5";
        //        var response = await _httpClient.GetAsync(url);

        //        List<ProductVariantViewModel> variants = new List<ProductVariantViewModel>();
        //        int totalItems = 0;

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            var result = JsonDocument.Parse(content);

        //            var json = result.RootElement.GetProperty("data").GetRawText();
        //            variants = JsonSerializer.Deserialize<List<ProductVariantViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //            totalItems = result.RootElement.GetProperty("totalItems").GetInt32();
        //        }
        //        else if (response.StatusCode == HttpStatusCode.NotFound)
        //        {
        //            variants = new List<ProductVariantViewModel>();
        //        }
        //        else
        //        {
        //            return View("Error");
        //        }

        //        int totalPages = (int)Math.Ceiling(totalItems / 5.0);

        //        ViewBag.CurrentPage = page;
        //        ViewBag.TotalPages = totalPages;
        //        ViewBag.SelectedProductName = productName;
        //        ViewBag.SelectedColor = color;
        //        ViewBag.SelectedVersion = version;
        //        ViewBag.IsSearch = true;

        //        ViewBag.Colors = await GetColorSelectList();
        //        ViewBag.Versions = await GetVersionSelectList();

        //        return View("~/Views/Staff/ProductVariants/Index.cshtml", variants);
        //    }
        //    catch
        //    {
        //        return View("Error");
        //    }
        //}

        private async Task<List<SelectListItem>> GetColorSelectList()
        {
            try
            {
                var response = await _httpClient.GetAsync("colors");
                var content = await response.Content.ReadAsStringAsync();
                var colors = JsonSerializer.Deserialize<List<ColorViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return colors.Select(c => new SelectListItem { Value = c.Name, Text = c.Name }).ToList();
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
                var versions = JsonSerializer.Deserialize<List<VersionViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return versions.Select(v => new SelectListItem { Value = v.Name, Text = v.Name }).ToList();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }
    }
}
