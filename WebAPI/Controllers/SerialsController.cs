using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using PhoneStoreAPI.Models;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SerialsController : ControllerBase
    {
        private readonly ISerialService _serialService;

        public SerialsController(ISerialService serialService)
        {
            _serialService = serialService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var serials = await _serialService.GetAllAsync();
            return Ok(serials);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var serial = await _serialService.GetByIdAsync(id);
            if (serial == null)
                return NotFound("Serial not found");
            return Ok(serial);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string SerialNumber)
        {
            var serials = await _serialService.SearchAsync(SerialNumber);
            return Ok(serials);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SerialDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra trùng SerialNumber
            var existing = await _serialService.SearchAsync(dto.SerialNumber);
            if (existing.Any(s => s.SerialNumber == dto.SerialNumber))
                return BadRequest("Đã trùng Serial Number có sẵn");

            var serial = new Serial
            {
                ProductVariantId = dto.ProductVariantId,
                SerialNumber = dto.SerialNumber,
                Status = dto.Status
            };

            await _serialService.AddAsync(serial);

            dto.Id = serial.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SerialDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existingSerial = await _serialService.GetByIdAsync(id);
            if (existingSerial == null)
                return NotFound();

            // Kiểm tra SerialNumber trùng với serial khác (loại trừ chính nó)
            var duplicates = await _serialService.SearchAsync(dto.SerialNumber);
            if (duplicates.Any(s => s.SerialNumber == dto.SerialNumber && s.Id != dto.Id))
                return BadRequest("Đã trùng Serial Number có sẵn");

            existingSerial.ProductVariantId = dto.ProductVariantId;
            existingSerial.SerialNumber = dto.SerialNumber;
            existingSerial.Status = dto.Status;

            await _serialService.UpdateAsync(existingSerial);
            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingSerial = await _serialService.GetByIdAsync(id);
            if (existingSerial == null)
                return NotFound();

            await _serialService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }
    }
}
