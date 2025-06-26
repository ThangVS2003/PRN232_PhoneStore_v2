using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using PhoneStoreAPI.Models;
using Service.IService;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var comments = await _commentService.GetAllAsync();
            var dtos = comments.Select(c => new CommentDto
            {
                Id = c.Id,
                UserId = c.UserId,
                ProductId = c.ProductId,
                Content = c.Content,
                ReplyId = c.ReplyId
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null)
                return NotFound("Comment not found");

            var dto = new CommentDto
            {
                Id = comment.Id,
                UserId = comment.UserId,
                ProductId = comment.ProductId,
                Content = comment.Content,
                ReplyId = comment.ReplyId
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = new Comment
            {
                UserId = dto.UserId,
                ProductId = dto.ProductId,
                Content = dto.Content,
                ReplyId = dto.ReplyId
            };

            await _commentService.AddAsync(comment);
            dto.Id = comment.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CommentDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existing = await _commentService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Comment not found");

            existing.UserId = dto.UserId;
            existing.ProductId = dto.ProductId;
            existing.Content = dto.Content;
            existing.ReplyId = dto.ReplyId;

            await _commentService.UpdateAsync(existing);

            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _commentService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Comment not found");

            await _commentService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }
    }
}
