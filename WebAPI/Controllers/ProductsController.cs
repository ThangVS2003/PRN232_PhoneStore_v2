using Microsoft.AspNetCore.Mvc;
using BusinessObject.Models;
using Service.IService;
using System.Threading.Tasks;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var result = new
            {
                product.Id,
                product.Name,
                product.Description,
                product.MainImage,
                product.BrandId,
                product.IsDeleted,
                Variants = product.ProductVariants
                    .Where(v => v.IsDeleted == false)
                    .Select(v => new
                    {
                        v.Id, // Include Id
                        Color = v.Color?.Name,
                        Version = v.Version?.Name,
                        v.SellingPrice,
                        v.OriginalPrice,
                        v.StockQuantity,
                        v.Image
                    }),
                    Feedbacks = product.FeedbackProducts
                    .Select(f => new
                    {
                        f.Comment,
                        f.Rating,
                        f.CreatedAt,
                        Username = f.User?.Name
                    })
            };

            return Ok(result);
        }

        // Other actions remain unchanged
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("by-brand/{brandId}")]
        public async Task<IActionResult> GetByBrandId(int brandId)
        {
            var products = await _productService.GetByBrandIdAsync(brandId);
            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] int? brandId, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var products = await _productService.SearchAsync(name, brandId, minPrice, maxPrice);
            return Ok(products);
        }

        [HttpGet("by-color")]
        public async Task<IActionResult> GetProductsByColor([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Color name is required.");

            var products = await _productService.GetByColorNameAsync(name);

            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.MainImage,
                Variants = p.ProductVariants
                    .Where(v => v.Color?.Name == name && v.IsDeleted == false)
                    .Select(v => new
                    {
                        v.Id,
                        v.OriginalPrice,
                        v.SellingPrice,
                        Color = v.Color?.Name,
                        Version = v.Version?.Name,
                        v.StockQuantity,
                        v.Image
                    })
            });

            return Ok(result);
        }

        [HttpGet("serial/{serialNumber}")]
        public async Task<IActionResult> GetBySerialNumber(string serialNumber)
        {
            var product = await _productService.GetBySerialNumberAsync(serialNumber);
            if (product == null)
                return NotFound($"No product found for serial number: {serialNumber}");
            return Ok(product);
        }

        [HttpGet("version-name/{versionName}")]
        public async Task<IActionResult> GetProductsByVersionName(string versionName)
        {
            var products = await _productService.GetProductsByVersionNameAsync(versionName);
            if (products == null || !products.Any())
                return NotFound($"No products found for version: {versionName}");
            return Ok(products);
        }
    }
}