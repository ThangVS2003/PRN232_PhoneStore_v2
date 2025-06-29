using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Models;
using Service.IService;
using System.Threading.Tasks;
using PhoneStoreAPI.Models;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found");
            return Ok(user);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string username)
        {
            var users = await _userService.SearchAsync(username);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _userService.GetByUsernameAsync(dto.Username);
            if (existing != null)
                return BadRequest("Username đã tồn tại");

            var user = new User
            {
                Username = dto.Username,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                Password = dto.Password,
                Role = dto.Role,
                Money = 0,
                IsDeleted = false
            };

            await _userService.AddAsync(user);
            dto.Id = user.Id;

            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
                return NotFound("User không tồn tại");

            var userWithSameUsername = await _userService.GetByUsernameAsync(dto.Username);
            if (userWithSameUsername != null && userWithSameUsername.Id != id)
                return BadRequest("Username đã tồn tại");

            existingUser.Username = dto.Username;
            existingUser.Name = dto.Name;
            existingUser.Email = dto.Email;
            existingUser.Phone = dto.Phone;
            existingUser.Address = dto.Address;
            existingUser.Password = dto.Password;
            existingUser.Role = dto.Role;
            existingUser.Money = dto.Money;

            await _userService.UpdateAsync(existingUser);
            return Ok("Đã sửa thành công");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
                return NotFound("User không tồn tại");

            await _userService.DeleteAsync(id);
            return Ok("Đã xóa thành công");
        }
    }
}