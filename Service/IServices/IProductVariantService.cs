using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IProductVariantService
    {
        Task<List<ProductVariant>> GetAllAsync();
        Task<ProductVariant> GetByIdAsync(int id);
        Task<List<ProductVariant>> GetByProductIdAsync(int productId);
        Task AddAsync(ProductVariant productVariant);
        Task UpdateAsync(ProductVariant productVariant);
        Task DeleteAsync(int id);
    }
}