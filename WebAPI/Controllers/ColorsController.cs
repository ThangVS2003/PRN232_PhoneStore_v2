using Microsoft.AspNetCore.Mvc;
using PhoneStoreAPI.Models;
using Service.IService;
using BusinessObject.Models;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly IColorService _colorService;

        public ColorsController(IColorService colorService)
        {
            _colorService = colorService;
        }

        // GET: api/colors?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool isPaging = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var colors = await _colorService.GetAllAsync();

            if (!isPaging)
            {
                // Trả về tất cả màu
                return Ok(colors.Select(c => new { c.Id, c.Name }));
            }

            // Trả về có phân trang
            int totalItems = colors.Count;
            var pagedColors = colors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                Data = pagedColors.Select(c => new { c.Id, c.Name }),
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var color = await _colorService.GetByIdAsync(id);
            if (color == null)
                return NotFound("Color not found");
            return Ok(color);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var colors = await _colorService.SearchAsync(name);

            int totalItems = colors.Count;
            var pagedColors = colors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                Data = pagedColors.Select(c => new { c.Id, c.Name }),
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ColorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra trùng Name
            var existing = await _colorService.SearchAsync(dto.Name);
            if (existing.Any(c => c.Name.ToLower() == dto.Name.ToLower()))
                return BadRequest("Đã trùng tên Color có sẵn");

            var color = new Color
            {
                Name = dto.Name
            };

            await _colorService.AddAsync(color);
            dto.Id = color.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ColorDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existingColor = await _colorService.GetByIdAsync(id);
            if (existingColor == null)
                return NotFound();

            // Kiểm tra Name trùng với Color khác (trừ chính nó)
            var duplicates = await _colorService.SearchAsync(dto.Name);
            if (duplicates.Any(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != dto.Id))
                return BadRequest("Đã trùng tên Color có sẵn");

            existingColor.Name = dto.Name;
            await _colorService.UpdateAsync(existingColor);

            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingColor = await _colorService.GetByIdAsync(id);
            if (existingColor == null)
                return NotFound();

            await _colorService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }
    }

}
