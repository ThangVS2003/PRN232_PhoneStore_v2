using BusinessObject.Models;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _productVariantRepository;

        public ProductVariantService(IProductVariantRepository productVariantRepository)
        {
            _productVariantRepository = productVariantRepository;
        }

        public async Task<List<ProductVariant>> GetAllAsync()
        {
            return await _productVariantRepository.GetAllAsync();
        }

        public async Task<ProductVariant> GetByIdAsync(int id)
        {
            return await _productVariantRepository.GetByIdAsync(id);
        }

        public async Task<List<ProductVariant>> SearchAsync(string productName, string color, string version)
        {
            return await _productVariantRepository.SearchAsync(productName, color, version);
        }

        public async Task<List<ProductVariant>> GetByProductIdAsync(int productId)
        {
            return await _productVariantRepository.GetByProductIdAsync(productId);
        }

        public async Task AddAsync(ProductVariant productVariant)
        {
            await _productVariantRepository.AddAsync(productVariant);
        }

        public async Task UpdateAsync(ProductVariant productVariant)
        {
            await _productVariantRepository.UpdateAsync(productVariant);
        }

        public async Task<bool> HasOrderDetailAsync(int id)
        {
            return await _productVariantRepository.HasOrderDetailAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            await _productVariantRepository.DeleteAsync(id);
        }

        public async Task RestoreAsync(int id)
        {
            await _productVariantRepository.RestoreAsync(id);
        }
    }
}
