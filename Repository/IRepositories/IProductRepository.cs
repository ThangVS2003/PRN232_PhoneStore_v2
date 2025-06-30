using BusinessObject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> SearchAsync(string? name, int? brandId, decimal? minPrice, decimal? maxPrice);
        Task<List<Product>> GetByBrandIdAsync(int brandId);
        Task<List<Product>> GetByColorNameAsync(string colorName);
        Task<Product?> GetBySerialNumberAsync(string serialNumber);
        Task<List<Product>> GetProductsByVersionNameAsync(string versionName);



        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task<Brand?> GetBrandByNameAsync(string brandName);

        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
        Task<List<Product>> GetByNameAndBrandIdAsync(string name, int brandId);

        Task<List<Product>> GetAllIncludeDeletedAsync();
        Task<Product> GetByIdIncludeDeletedAsync(int id);
    }
}