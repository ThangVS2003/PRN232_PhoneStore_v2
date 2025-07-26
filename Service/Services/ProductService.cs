using BusinessObject.Models;
using Repository.IRepository;
using Service.IService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetAllAsync(bool includeDeleted = false)
        {
            return await _productRepository.GetAllAsync(includeDeleted);
        }

        public async Task<Product> GetByIdAsync(int id, bool includeDeleted = false)
        {
            return await _productRepository.GetByIdAsync(id, includeDeleted);
        }

        public async Task<List<Product>> SearchAsync(string? name, int? brandId, int? versionId, decimal? minPrice, decimal? maxPrice)
        {
            return await _productRepository.SearchAsync(name, brandId, versionId, minPrice, maxPrice);
        }

        public async Task<List<Product>> GetByBrandIdAsync(int brandId)
        {
            return await _productRepository.GetByBrandIdAsync(brandId);
        }
        public async Task<List<Product>> GetByVersionIdAsync(int versionId)
        {
            return await _productRepository.GetByVersionIdAsync(versionId);
        }

        public async Task<List<Product>> GetByColorNameAsync(string colorName)
        {
            return await _productRepository.GetByColorNameAsync(colorName);
        }
        public async Task<Product?> GetBySerialNumberAsync(string serialNumber)
        {
            return await _productRepository.GetBySerialNumberAsync(serialNumber);
        }
        public async Task<List<Product>> GetProductsByVersionNameAsync(string versionName)
        {
            return await _productRepository.GetProductsByVersionNameAsync(versionName);
        }



        public async Task AddAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        public async Task<Brand?> GetBrandByNameAsync(string brandName)
        {
            return await _productRepository.GetBrandByNameAsync(brandName);
        }

        public async Task DeleteAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }

        public async Task RestoreAsync(int id)
        {
            await _productRepository.RestoreAsync(id); // Gọi repository
        }

        public async Task<List<Product>> GetByNameAndBrandIdAsync(string name, int brandId)
        {
            return await _productRepository.GetByNameAndBrandIdAsync(name, brandId);
        }

        public async Task<List<Product>> SearchByNameAsync(string keyword)
        {
            return await _productRepository.SearchByNameAsync(keyword);
        }
    }
}