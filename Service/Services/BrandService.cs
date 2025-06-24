// Service/Service/BrandService.cs
using BusinessObject.Models;
using Repository.IRepository;
using Repository.Repository;
using Service.IService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;

        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _brandRepository.GetAllAsync();
        }

        public async Task<Brand> GetByIdAsync(int id)
        {
            return await _brandRepository.GetByIdAsync(id);
        }

        public async Task<List<Brand>> SearchAsync(string name)
        {
            return await _brandRepository.SearchAsync(name);
        }

        public async Task AddAsync(Brand brand)
        {
            await _brandRepository.AddAsync(brand);
        }

        public async Task UpdateAsync(Brand brand)
        {
            await _brandRepository.UpdateAsync(brand);
        }

        public async Task DeleteAsync(int id)
        {
            await _brandRepository.DeleteAsync(id);
        }
    }
}