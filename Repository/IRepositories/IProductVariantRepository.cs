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
        Task<List<ProductVariant>> SearchAsync(string productName, string color, string version);
        Task<List<ProductVariant>> GetByProductIdAsync(int productId);
        Task AddAsync(ProductVariant productVariant);
        Task UpdateAsync(ProductVariant productVariant);
        Task<bool> HasOrderDetailAsync(int productVariantId);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
    }
}
