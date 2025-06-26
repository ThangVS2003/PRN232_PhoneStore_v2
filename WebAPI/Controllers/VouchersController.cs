using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using PhoneStoreAPI.Models;
using Service.IService;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var vouchers = await _voucherService.GetAllAsync();
            return Ok(vouchers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null)
                return NotFound("Voucher not found");

            return Ok(voucher);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string code)
        {
            var vouchers = await _voucherService.SearchAsync(code);
            return Ok(vouchers);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VoucherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra trùng Code
            var existing = await _voucherService.SearchAsync(dto.Code);
            if (existing.Any(v => v.Code.ToLower() == dto.Code.ToLower()))
                return BadRequest("Đã trùng mã Voucher có sẵn");

            var voucher = new Voucher
            {
                Code = dto.Code,
                DiscountValue = dto.DiscountValue,
                DiscountType = dto.DiscountType,
                MinOrderValue = dto.MinOrderValue,
                ExpiryDate = DateTime.UtcNow.AddMonths(1), // Set giá trị giả định, có thể thêm vào Dto nếu cần
                IsActive = dto.IsActive,
                ApplyType = dto.ApplyType,
                Description = dto.Description
            };

            await _voucherService.AddAsync(voucher);
            dto.Id = voucher.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VoucherDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existingVoucher = await _voucherService.GetByIdAsync(id);
            if (existingVoucher == null)
                return NotFound();

            // Kiểm tra Code trùng với Voucher khác (trừ chính nó)
            var duplicates = await _voucherService.SearchAsync(dto.Code);
            if (duplicates.Any(v => v.Code.ToLower() == dto.Code.ToLower() && v.Id != dto.Id))
                return BadRequest("Đã trùng mã Voucher có sẵn");

            existingVoucher.Code = dto.Code;
            existingVoucher.DiscountValue = dto.DiscountValue;
            existingVoucher.DiscountType = dto.DiscountType;
            existingVoucher.MinOrderValue = dto.MinOrderValue;
            existingVoucher.IsActive = dto.IsActive;
            existingVoucher.ApplyType = dto.ApplyType;
            existingVoucher.Description = dto.Description;

            await _voucherService.UpdateAsync(existingVoucher);

            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingVoucher = await _voucherService.GetByIdAsync(id);
            if (existingVoucher == null)
                return NotFound();

            await _voucherService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }
    }
}
