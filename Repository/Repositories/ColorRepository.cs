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
    public class ColorRepository : IColorRepository
    {
        private readonly DbPhoneStoreContext _context;

        public ColorRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Color>> GetAllAsync()
        {
            return await _context.Colors
                .Include(c => c.ProductVariants) // Include if you want to use navigation
                .ToListAsync();
        }

        public async Task<Color?> GetByIdAsync(int id)
        {
            return await _context.Colors
                .Include(c => c.ProductVariants)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Color>> SearchAsync(string keyword)
        {
            var query = _context.Colors
                .Include(c => c.ProductVariants)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.Name.Contains(keyword));
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(Color color)
        {
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Color color)
        {
            var existingColor = await _context.Colors.FindAsync(color.Id);
            if (existingColor != null)
            {
                existingColor.Name = color.Name;

                _context.Colors.Update(existingColor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color != null)
            {
                _context.Colors.Remove(color);
                await _context.SaveChangesAsync();
            }
        }
    }

}
