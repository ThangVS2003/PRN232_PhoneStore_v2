using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using PhoneStoreAPI.Models;
using Service.IService;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackProductsController : ControllerBase
    {
        private readonly IFeedbackProductService _feedbackProductService;

        public FeedbackProductsController(IFeedbackProductService feedbackProductService)
        {
            _feedbackProductService = feedbackProductService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var feedbacks = await _feedbackProductService.GetAllAsync();
            var dtos = feedbacks.Select(f => new FeedbackProductDto
            {
                Id = f.Id,
                UserId = f.UserId,
                ProductId = f.ProductId,
                Rating = f.Rating,
                Comment = f.Comment
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var feedback = await _feedbackProductService.GetByIdAsync(id);
            if (feedback == null)
                return NotFound("Feedback not found");

            var dto = new FeedbackProductDto
            {
                Id = feedback.Id,
                UserId = feedback.UserId,
                ProductId = feedback.ProductId,
                Rating = feedback.Rating,
                Comment = feedback.Comment
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeedbackProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var feedback = new FeedbackProduct
            {
                UserId = dto.UserId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _feedbackProductService.AddAsync(feedback);
            dto.Id = feedback.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FeedbackProductDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existing = await _feedbackProductService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Feedback not found");

            existing.UserId = dto.UserId;
            existing.ProductId = dto.ProductId;
            existing.Rating = dto.Rating;
            existing.Comment = dto.Comment;

            await _feedbackProductService.UpdateAsync(existing);

            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _feedbackProductService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Feedback not found");

            await _feedbackProductService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var feedbacks = await _feedbackProductService.GetByProductIdAsync(productId);
            if (feedbacks == null || !feedbacks.Any())
                return NotFound("No feedback found for the given product.");

            var dtos = feedbacks.Select(f => new FeedbackProductDto
            {
                Id = f.Id,
                UserName = f.User.Name,
                ProductId = f.ProductId,
                Rating = f.Rating,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt
            }).ToList();

            return Ok(dtos);
        }
    }
}