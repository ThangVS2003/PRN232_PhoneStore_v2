// Repository/Repository/UserRepository.cs
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DbPhoneStoreContext _context;

        public UserRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Where(u => u.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == false);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsDeleted == false);
        }

        public async Task<List<User>> SearchAsync(string username, string email, int? role)
        {
            var query = _context.Users.Where(u => u.IsDeleted == false);
            if (!string.IsNullOrEmpty(username))
                query = query.Where(u => u.Username.Contains(username));
            if (!string.IsNullOrEmpty(email))
                query = query.Where(u => u.Email.Contains(email));
            if (role.HasValue)
                query = query.Where(u => u.Role == role);
            return await query.ToListAsync();
        }
    }
}