using Microsoft.AspNetCore.Mvc;
using PhoneStoreAPI.Models;
using Service.IService;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionsController : ControllerBase
    {
        private readonly IVersionService _versionService;

        public VersionsController(IVersionService versionService)
        {
            _versionService = versionService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var versions = await _versionService.GetAllAsync();
            return Ok(versions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var version = await _versionService.GetByIdAsync(id);
            if (version == null)
                return NotFound("Version not found");
            return Ok(version);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var versions = await _versionService.SearchAsync(name);
            return Ok(versions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VersionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra trùng Name
            var existing = await _versionService.SearchAsync(dto.Name);
            if (existing.Any(v => v.Name.ToLower() == dto.Name.ToLower()))
                return BadRequest("Đã trùng tên Version có sẵn");

            var version = new BusinessObject.Models.Version
            {
                Name = dto.Name
            };

            await _versionService.AddAsync(version);
            dto.Id = version.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VersionDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existingVersion = await _versionService.GetByIdAsync(id);
            if (existingVersion == null)
                return NotFound();

            // Kiểm tra trùng tên Version khác (trừ chính nó)
            var duplicates = await _versionService.SearchAsync(dto.Name);
            if (duplicates.Any(v => v.Name.ToLower() == dto.Name.ToLower() && v.Id != dto.Id))
                return BadRequest("Đã trùng tên Version có sẵn");

            existingVersion.Name = dto.Name;
            await _versionService.UpdateAsync(existingVersion);

            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingVersion = await _versionService.GetByIdAsync(id);
            if (existingVersion == null)
                return NotFound();

            await _versionService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }
    }
}
