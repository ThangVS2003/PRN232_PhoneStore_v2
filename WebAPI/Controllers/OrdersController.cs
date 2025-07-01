using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using PhoneStoreAPI.Models;
using Service.IService;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound("Order not found");

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = new Order
            {
                UserId = dto.UserId,
                TotalAmount = dto.TotalAmount,
                Status = dto.Status,
                ShippingAddress = dto.ShippingAddress,
                VoucherId = dto.VoucherId,
                OrderDate = DateTime.UtcNow
            };

            await _orderService.AddAsync(order);
            dto.Id = order.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existingOrder = await _orderService.GetByIdAsync(id);
            if (existingOrder == null)
                return NotFound();

            existingOrder.UserId = dto.UserId;
            existingOrder.TotalAmount = dto.TotalAmount;
            existingOrder.Status = dto.Status;
            existingOrder.ShippingAddress = dto.ShippingAddress;
            existingOrder.VoucherId = dto.VoucherId;

            await _orderService.UpdateAsync(existingOrder);

            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingOrder = await _orderService.GetByIdAsync(id);
            if (existingOrder == null)
                return NotFound();

            await _orderService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            if (orders == null || !orders.Any())
                return NotFound("Không có đơn hàng nào");
            var result = orders.Select(o => new
            {
                o.Id,
                o.OrderDate,
                o.Status,
                o.TotalAmount,
                o.ShippingAddress
            });

            return Ok(result);
        }

        [HttpGet("details/{orderId}")]
        public async Task<IActionResult> GetOrderDetailsById(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null)
                return NotFound("Đơn hàng không tồn tại");

            var result = new
            {
                order.Id,
                order.OrderDate,
                order.Status,
                order.TotalAmount,
                order.ShippingAddress,
                Products = order.OrderDetails.Select(od => new
                {
                    ProductName = od.ProductVariant.Product.Name,
                    Version = od.ProductVariant.Version?.Name,
                    Color = od.ProductVariant.Color?.Name,
                    od.Quantity,
                    od.UnitPrice,
                    Image = od.ProductVariant.Image
                })
            };

            return Ok(result);
        }

    }
}
