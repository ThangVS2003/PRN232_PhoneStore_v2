using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbPhoneStoreContext _context;

        public ProductRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var products = await _context.Products
                .Where(p => p.IsDeleted == false)
                .ToListAsync();
            Console.WriteLine($"GetAllAsync: Found {products.Count} products");
            return products;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
            Console.WriteLine($"GetByIdAsync: ProductId {id} {(product != null ? "found" : "not found")}");
            return product;
        }

        public async Task<List<Product>> SearchAsync(string? name, int? brandId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products
                .Where(p => p.IsDeleted == false);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));
            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId);

            var products = await query.ToListAsync();
            Console.WriteLine($"SearchAsync: Found {products.Count} products for brandId={brandId}, name={name}");
            return products;
        }

        public async Task<List<Product>> GetByBrandIdAsync(int brandId)
        {
            var products = await _context.Products
                .Where(p => p.BrandId == brandId && p.IsDeleted == false)
                .ToListAsync();
            Console.WriteLine($"GetByBrandIdAsync: Found {products.Count} products for brandId={brandId}");
            return products;
        }
    }
}