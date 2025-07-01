using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IProductVariantRepository
    {
        Task<List<ProductVariant>> GetAllAsync();
        Task<ProductVariant?> GetByIdAsync(int id);
        Task<List<ProductVariant>> GetByProductIdAsync(int productId);
        Task AddAsync(ProductVariant productVariant);
        Task UpdateAsync(ProductVariant productVariant);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
    }
}
