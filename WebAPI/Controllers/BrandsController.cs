// PhoneStoreAPI/Controllers/BrandsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Models;
using Service.IService;
using System.Threading.Tasks;
using PhoneStoreAPI.Models;
namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,Staff")]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var brands = await _brandService.GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand == null)
                return NotFound("Brand not found");
            return Ok(brand);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var brands = await _brandService.SearchAsync(name);
            return Ok(brands);
        }





        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BrandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var brand = new Brand
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _brandService.AddAsync(brand);

            dto.Id = brand.Id; // Gán lại id sau khi lưu

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existingBrand = await _brandService.GetByIdAsync(id);
            if (existingBrand == null)
                return NotFound();

            // Cập nhật 3 thuộc tính
            existingBrand.Name = dto.Name;
            existingBrand.Description = dto.Description;

            await _brandService.UpdateAsync(existingBrand);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingBrand = await _brandService.GetByIdAsync(id);
            if (existingBrand == null)
                return NotFound();

            await _brandService.DeleteAsync(id);
            return NoContent();
        }
    }
}