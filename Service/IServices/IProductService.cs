using BusinessObject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync(bool includeDeleted = false);
        Task<Product> GetByIdAsync(int id, bool includeDeleted = false);
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
        Task<List<Product>> SearchByNameAsync(string keyword);
    }
}