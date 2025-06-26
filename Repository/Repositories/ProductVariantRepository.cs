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
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly DbPhoneStoreContext _context;

        public ProductVariantRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<ProductVariant>> GetAllAsync()
        {
            return await _context.Set<ProductVariant>()
                .Where(pv => pv.IsDeleted != true)
                .Include(pv => pv.Color)
                .Include(pv => pv.Version)
                .Include(pv => pv.Product)
                .ToListAsync();
        }

        public async Task<ProductVariant?> GetByIdAsync(int id)
        {
            return await _context.Set<ProductVariant>()
                .Include(pv => pv.Color)
                .Include(pv => pv.Version)
                .Include(pv => pv.Product)
                .FirstOrDefaultAsync(pv => pv.Id == id && pv.IsDeleted != true);
        }

        public async Task AddAsync(ProductVariant productVariant)
        {
            productVariant.CreatedAt = DateTime.Now;
            _context.Set<ProductVariant>().Add(productVariant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductVariant productVariant)
        {
            productVariant.UpdatedAt = DateTime.Now;
            _context.Set<ProductVariant>().Update(productVariant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var productVariant = await _context.Set<ProductVariant>().FindAsync(id);
            if (productVariant != null)
            {
                productVariant.IsDeleted = true;
                productVariant.DeletedAt = DateTime.Now;
                _context.Set<ProductVariant>().Update(productVariant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
