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
    }
}