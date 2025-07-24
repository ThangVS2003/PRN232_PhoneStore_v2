using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using PhoneStoreAPI.Models;
using Service.IService;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantsController : ControllerBase
    {
        private readonly IProductVariantService _productVariantService;

        public ProductVariantsController(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    var variants = await _productVariantService.GetAllAsync();

        //    var dtos = variants.Select(v => new ProductVariantDto
        //    {
        //        Id = v.Id,
        //        ProductId = v.ProductId,
        //        ProductName = v.Product.Name,
        //        ColorId = v.ColorId,
        //        VersionId = v.VersionId,
        //        OriginalPrice = v.OriginalPrice,
        //        SellingPrice = v.SellingPrice,
        //        StockQuantity = v.StockQuantity,
        //        Image = v.Image
        //    }).ToList();

        //    return Ok(dtos);
        //}

        // GET: api/ProductVariants?page=1&pageSize=5
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var variants = await _productVariantService.GetAllAsync();
            var totalItems = variants.Count;

            var paged = variants
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new
                {
                    v.Id,
                    v.ProductId,
                    ProductName = v.Product.Name,
                    Color = v.Color?.Name ?? "Unknown",
                    Version = v.Version?.Name ?? "Unknown",
                    v.SellingPrice,
                    v.OriginalPrice,
                    v.StockQuantity,
                    v.Image,
                    v.IsDeleted
                });

            return Ok(new
            {
                Data = paged,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var variant = await _productVariantService.GetByIdAsync(id);
            if (variant == null)
                return NotFound("Product variant not found");

            return Ok(variant);
        }

        // GET: api/ProductVariants/search? productName = ...&color=...&version=...&page=1&pageSize=5
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string productName, [FromQuery] string color, [FromQuery] string version, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var variants = await _productVariantService.SearchAsync(productName, color, version);
            var totalItems = variants.Count;

            var paged = variants
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new
                {
                    v.Id,
                    v.ProductId,
                    ProductName = v.Product.Name,
                    Color = v.Color?.Name ?? "Unknown",
                    Version = v.Version?.Name ?? "Unknown",
                    v.SellingPrice,
                    v.OriginalPrice,
                    v.StockQuantity,
                    v.Image,
                    v.IsDeleted
                });

            return Ok(new
            {
                Data = paged,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var variants = await _productVariantService.GetByProductIdAsync(productId);
            if (variants == null || !variants.Any())
                return NotFound($"No product variants found for product ID {productId}");

            var dtos = variants.Select(v => new ProductVariantDto
            {
                Id = v.Id,
                ProductId = v.ProductId,
                ColorId = v.ColorId,
                VersionId = v.VersionId,
                OriginalPrice = v.OriginalPrice,
                SellingPrice = v.SellingPrice,
                StockQuantity = v.StockQuantity,
                Image = v.Image
            }).ToList();

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductVariantDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var variant = new ProductVariant
            {
                ProductId = dto.ProductId,
                ColorId = dto.ColorId,
                VersionId = dto.VersionId,
                OriginalPrice = dto.OriginalPrice,
                SellingPrice = dto.SellingPrice,
                StockQuantity = dto.StockQuantity,
                Image = dto.Image,
                //CreatedAt = DateTime.Now
                IsDeleted = false
            };

            await _productVariantService.AddAsync(variant);
            dto.Id = variant.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductVariantDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existing = await _productVariantService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Product variant not found");

            existing.ProductId = dto.ProductId;
            existing.ColorId = dto.ColorId;
            existing.VersionId = dto.VersionId;
            existing.OriginalPrice = dto.OriginalPrice;
            existing.SellingPrice = dto.SellingPrice;
            existing.StockQuantity = dto.StockQuantity;
            existing.Image = dto.Image;
            existing.UpdatedAt = DateTime.Now;

            await _productVariantService.UpdateAsync(existing);
            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _productVariantService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Product variant not found");

            var hasOrderDetail = await _productVariantService.HasOrderDetailAsync(id);
            if (hasOrderDetail)
                return BadRequest("Không thể xóa vì sản phẩm này đã tồn tại trong đơn hàng.");

            await _productVariantService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            await _productVariantService.RestoreAsync(id);
            return Ok("Khôi phục biến thể thành công");
        }
    }
}
