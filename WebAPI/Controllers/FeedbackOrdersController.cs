using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using PhoneStoreAPI.Models;
using Service.IService;
using Service.Service;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackOrdersController : ControllerBase
    {
        private readonly IFeedbackOrderService _feedbackOrderService;

        public FeedbackOrdersController(IFeedbackOrderService feedbackOrderService)
        {
            _feedbackOrderService = feedbackOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var feedbacks = await _feedbackOrderService.GetAllAsync();
            var result = feedbacks.Select(f => new FeedbackOrderDto
            {
                Id = f.Id,
                UserId = f.UserId,
                OrderId = f.OrderId,
                Rating = f.Rating,
                Comment = f.Comment
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var feedback = await _feedbackOrderService.GetByIdAsync(id);
            if (feedback == null)
                return NotFound("Feedback order not found");

            var dto = new FeedbackOrderDto
            {
                Id = feedback.Id,
                UserId = feedback.UserId,
                OrderId = feedback.OrderId,
                Rating = feedback.Rating,
                Comment = feedback.Comment
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeedbackOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var feedback = new FeedbackOrder
            {
                UserId = dto.UserId,
                OrderId = dto.OrderId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                //CreatedAt = DateTime.UtcNow
            };

            await _feedbackOrderService.AddAsync(feedback);
            dto.Id = feedback.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FeedbackOrderDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existing = await _feedbackOrderService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Feedback order not found");

            existing.UserId = dto.UserId;
            existing.OrderId = dto.OrderId;
            existing.Rating = dto.Rating;
            existing.Comment = dto.Comment;

            await _feedbackOrderService.UpdateAsync(existing);
            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _feedbackOrderService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Feedback order not found");

            await _feedbackOrderService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var feedbacks = await _feedbackOrderService.GetByOrderIdAsync(orderId);
            if (feedbacks == null || !feedbacks.Any())
                return NotFound("No feedback found for the given order.");

            var dtos = feedbacks.Select(f => new FeedbackOrderDto
            {
                Id = f.Id,
                UserName = f.User.Name,
                OrderId = f.OrderId,
                Rating = f.Rating,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt
            }).ToList();

            return Ok(dtos);
        }
    }
}
