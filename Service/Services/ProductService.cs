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
    }
}