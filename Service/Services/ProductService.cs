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

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<List<Product>> SearchAsync(string? name, int? brandId, decimal? minPrice, decimal? maxPrice)
        {
            return await _productRepository.SearchAsync(name, brandId, minPrice, maxPrice);
        }

        public async Task<List<Product>> GetByBrandIdAsync(int brandId)
        {
            return await _productRepository.GetByBrandIdAsync(brandId);
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

        public async Task<List<Product>> GetByNameAndBrandIdAsync(string name, int brandId)
        {
            return await _productRepository.GetByNameAndBrandIdAsync(name, brandId);
        }
    }
}