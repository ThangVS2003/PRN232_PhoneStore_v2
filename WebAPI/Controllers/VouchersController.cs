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

        // GET: api/vouchers?page=1&pageSize=10&isPaging=true
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool isPaging = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var vouchers = await _voucherService.GetAllAsync();

            if (!isPaging)
            {
                return Ok(vouchers);
            }

            int totalItems = vouchers.Count;
            var pagedVouchers = vouchers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                Data = pagedVouchers,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            });
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
        public async Task<IActionResult> Search([FromQuery] string code, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var vouchers = await _voucherService.SearchAsync(code);

            int totalItems = vouchers.Count;
            var pagedVouchers = vouchers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                Data = pagedVouchers,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            });
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
                ExpiryDate = dto.ExpiryDate,
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
            existingVoucher.ExpiryDate = dto.ExpiryDate;
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

        //[HttpPut("{id}/toggle-status")]
        //public async Task<IActionResult> ToggleStatus(int id)
        //{
        //    var success = await _voucherService.ToggleActiveStatusAsync(id);
        //    if (!success)
        //        return NotFound("Voucher not found or update failed");

        //    return Ok("Voucher status updated successfully");
        //}
    }
}
