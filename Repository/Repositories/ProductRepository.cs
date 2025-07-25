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

        public async Task<List<Product>> GetAllAsync(bool includeDeleted = false)
        {
            var query = _context.Products
                .Include(p => p.Brand)
                .AsQueryable();

            if (!includeDeleted)
                query = query.Where(p => p.IsDeleted == false);

            var products = await query.ToListAsync();
            Console.WriteLine($"GetAllAsync (includeDeleted={includeDeleted}): Found {products.Count} products");
            return products;
        }

        public async Task<Product> GetByIdAsync(int id, bool includeDeleted = false)
        {
            var query = _context.Products
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Color)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Version)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.OrderDetails)
                .Include(p => p.FeedbackProducts)
                    .ThenInclude(f => f.User)
                .AsQueryable();

            if (!includeDeleted)
                query = query.Where(p => p.IsDeleted == false);

            var product = await query.FirstOrDefaultAsync(p => p.Id == id);
            Console.WriteLine($"GetByIdAsync (id={id}, includeDeleted={includeDeleted}): {(product != null ? "found" : "not found")}");
            return product;
        }

        public async Task<List<Product>> SearchAsync(string? name, int? brandId, int? versionId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products
                .Where(p => p.IsDeleted == false);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId);

            if (versionId.HasValue)
                query = query.Where(p => p.ProductVariants.Any(v => v.VersionId == versionId && v.IsDeleted == false));

            // Nếu muốn, có thể lọc thêm theo minPrice, maxPrice trên ProductVariant:
            // Ví dụ:
            if (minPrice.HasValue)
                query = query.Where(p => p.ProductVariants.Any(v => v.SellingPrice >= minPrice && v.IsDeleted == false));
            if (maxPrice.HasValue)
                query = query.Where(p => p.ProductVariants.Any(v => v.SellingPrice <= maxPrice && v.IsDeleted == false));

            var products = await query.ToListAsync();

            Console.WriteLine($"SearchAsync: Found {products.Count} products for name={name}, brandId={brandId}, versionId={versionId}");
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
        public async Task<List<Product>> GetByVersionIdAsync(int versionId)
        {
            var products = await _context.Products
                .Where(p => p.IsDeleted == false &&
                    p.ProductVariants.Any(v => v.VersionId == versionId && v.IsDeleted == false))
                .ToListAsync();

            Console.WriteLine($"GetByVersionIdAsync: Found {products.Count} products for versionId={versionId}");
            return products;
        }

        public async Task<List<Product>> GetByColorNameAsync(string colorName)
        {
            return await _context.Products
        .Include(p => p.ProductVariants)
            .ThenInclude(v => v.Color)
        .Where(p => p.ProductVariants.Any(v => v.Color != null && v.Color.Name == colorName && v.IsDeleted == false))
        .Where(p => p.IsDeleted == false)
        .ToListAsync();
        }
        public async Task<Product?> GetBySerialNumberAsync(string serialNumber)
        {
            var product = await _context.Products
                .Where(p => p.IsDeleted != true &&
                            p.ProductVariants.Any(pv => pv.Serials.Any(s => s.SerialNumber == serialNumber)))
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Serials)
                .FirstOrDefaultAsync();

            if (product != null)
            {
               
                product.ProductVariants = product.ProductVariants
                    .Where(pv => pv.Serials.Any(s => s.SerialNumber == serialNumber))
                    .Select(pv =>
                    {
                  
                        pv.Serials = pv.Serials
                            .Where(s => s.SerialNumber == serialNumber)
                            .ToList();
                        return pv;
                    })
                    .ToList();
            }

            Console.WriteLine($"GetBySerialNumberAsync: SerialNumber = {serialNumber}, Found = {product != null}");
            return product;
        }
        public async Task<List<Product>> GetProductsByVersionNameAsync(string versionName)
        {
            var products = await _context.Products
                .Where(p => p.IsDeleted != true &&
                            p.ProductVariants.Any(pv => pv.Version != null && pv.Version.Name == versionName))
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Version)
                .ToListAsync();

            return products;
        }



        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Brand?> GetBrandByNameAsync(string brandName)
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.Name == brandName);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                product.DeletedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RestoreAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null && product.IsDeleted == true)
            {
                product.IsDeleted = false;
                product.DeletedAt = null;
                product.DeletedBy = null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> GetByNameAndBrandIdAsync(string name, int brandId)
        {
            var query = _context.Products
                .Where(p => p.IsDeleted == false || p.IsDeleted == true);

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                query = query.Where(p => p.Name.Contains(name));
            }

            if (brandId != 0)
                query = query.Where(p => p.BrandId == brandId);

            return await query
                .Include(p => p.Brand)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchByNameAsync(string keyword)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(p => p.Name.Contains(keyword));
            }

            return await query.ToListAsync();
        }
    }
}