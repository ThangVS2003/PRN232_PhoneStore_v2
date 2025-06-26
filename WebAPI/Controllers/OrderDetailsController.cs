using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using PhoneStoreAPI.Models;
using Service.IService;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailsController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orderDetails = await _orderDetailService.GetAllAsync();
            return Ok(orderDetails);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var orderDetail = await _orderDetailService.GetByIdAsync(id);
            if (orderDetail == null)
                return NotFound("OrderDetail not found");

            return Ok(orderDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderDetailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderDetail = new OrderDetail
            {
                OrderId = dto.OrderId,
                ProductVariantId = dto.ProductVariantId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };

            await _orderDetailService.AddAsync(orderDetail);
            dto.Id = orderDetail.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDetailDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existing = await _orderDetailService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.OrderId = dto.OrderId;
            existing.ProductVariantId = dto.ProductVariantId;
            existing.Quantity = dto.Quantity;
            existing.UnitPrice = dto.UnitPrice;

            await _orderDetailService.UpdateAsync(existing);

            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _orderDetailService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _orderDetailService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }
    }
}