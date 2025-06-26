using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using PhoneStoreAPI.Models;
using Service.IService;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositHistoriesController : ControllerBase
    {
        private readonly IDepositHistoryService _depositHistoryService;

        public DepositHistoriesController(IDepositHistoryService depositHistoryService)
        {
            _depositHistoryService = depositHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var deposits = await _depositHistoryService.GetAllAsync();
            var dtos = deposits.Select(d => new DepositHistoryDto
            {
                Id = d.Id,
                UserId = d.UserId,
                Amount = d.Amount,
                PaymentMethod = d.PaymentMethod,
                Status = d.Status
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var deposit = await _depositHistoryService.GetByIdAsync(id);
            if (deposit == null)
                return NotFound("Deposit history not found");

            var dto = new DepositHistoryDto
            {
                Id = deposit.Id,
                UserId = deposit.UserId,
                Amount = deposit.Amount,
                PaymentMethod = deposit.PaymentMethod,
                Status = deposit.Status
            };

            return Ok(dto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string paymentMethod)
        {
            var deposits = await _depositHistoryService.SearchAsync(paymentMethod);
            var dtos = deposits.Select(d => new DepositHistoryDto
            {
                Id = d.Id,
                UserId = d.UserId,
                Amount = d.Amount,
                PaymentMethod = d.PaymentMethod,
                Status = d.Status
            }).ToList();

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepositHistoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deposit = new DepositHistory
            {
                UserId = dto.UserId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = dto.Status,
                //DepositDate = DateTime.UtcNow
            };

            await _depositHistoryService.AddAsync(deposit);
            dto.Id = deposit.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepositHistoryDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existing = await _depositHistoryService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Deposit history not found");

            existing.UserId = dto.UserId;
            existing.Amount = dto.Amount;
            existing.PaymentMethod = dto.PaymentMethod;
            existing.Status = dto.Status;

            await _depositHistoryService.UpdateAsync(existing);

            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _depositHistoryService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Deposit history not found");

            await _depositHistoryService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }
    }
}
