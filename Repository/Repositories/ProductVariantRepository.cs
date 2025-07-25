﻿using BusinessObject.Models;
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

        public async Task<List<ProductVariant>> SearchAsync(string productName, string color, string version)
        {
            var query = _context.ProductVariants
                .Include(v => v.Product)
                .Include(v => v.Color)
                .Include(v => v.Version)
                .AsQueryable();

            if (!string.IsNullOrEmpty(productName))
            {
                productName = productName.Trim();
                query = query.Where(v => v.Product.Name.Contains(productName));
            }

            if (!string.IsNullOrEmpty(color))
                query = query.Where(v => v.Color != null && v.Color.Name == color);

            if (!string.IsNullOrEmpty(version))
                query = query.Where(v => v.Version != null && v.Version.Name == version);

            return await query.ToListAsync();
        }

        public async Task<List<ProductVariant>> GetByProductIdAsync(int productId)
        {
            return await _context.Set<ProductVariant>()
                .Where(pv => pv.ProductId == productId && pv.IsDeleted != true)
                .Include(pv => pv.Color)
                .Include(pv => pv.Version)
                .Include(pv => pv.Product)
                .ToListAsync();
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

        public async Task<bool> HasOrderDetailAsync(int productVariantId)
        {
            return await _context.OrderDetails
                .AnyAsync(od => od.ProductVariantId == productVariantId);
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

        public async Task RestoreAsync(int id)
        {
            var productVariant = await _context.Set<ProductVariant>().FindAsync(id);
            if (productVariant != null && productVariant.IsDeleted == true)
            {
                productVariant.IsDeleted = false;
                productVariant.DeletedAt = null;
                productVariant.DeletedBy = null;
                _context.Set<ProductVariant>().Update(productVariant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
