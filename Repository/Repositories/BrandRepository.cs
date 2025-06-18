// Repository/Repository/BrandRepository.cs
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class BrandRepository : IBrandRepository
    {
        private readonly DbPhoneStoreContext _context;

        public BrandRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand> GetByIdAsync(int id)
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Brand>> SearchAsync(string name)
        {
            var query = _context.Brands.AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(b => b.Name.Contains(name));
            return await query.ToListAsync();
        }
    }
}