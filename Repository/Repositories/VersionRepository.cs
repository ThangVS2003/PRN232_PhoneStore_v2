using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class VersionRepository : IVersionRepository
    {
        private readonly DbPhoneStoreContext _context;

        public VersionRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<BusinessObject.Models.Version>> GetAllAsync()
        {
            return await _context.Versions
                .Include(v => v.ProductVariants) // Include navigation if needed
                .ToListAsync();
        }

        public async Task<BusinessObject.Models.Version?> GetByIdAsync(int id)
        {
            return await _context.Versions
                .Include(v => v.ProductVariants)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<BusinessObject.Models.Version>> SearchAsync(string keyword)
        {
            var query = _context.Versions
                .Include(v => v.ProductVariants)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(v => v.Name.Contains(keyword));
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(BusinessObject.Models.Version version)
        {
            await _context.Versions.AddAsync(version);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BusinessObject.Models.Version version)
        {
            var existingVersion = await _context.Versions.FindAsync(version.Id);
            if (existingVersion != null)
            {
                existingVersion.Name = version.Name;

                _context.Versions.Update(existingVersion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var version = await _context.Versions.FindAsync(id);
            if (version != null)
            {
                _context.Versions.Remove(version);
                await _context.SaveChangesAsync();
            }
        }
    }
}
