using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class VersionService : IVersionService
    {
        private readonly IVersionRepository _versionRepository;

        public VersionService(IVersionRepository versionRepository)
        {
            _versionRepository = versionRepository;
        }

        public async Task<List<BusinessObject.Models.Version>> GetAllAsync()
        {
            return await _versionRepository.GetAllAsync();
        }

        public async Task<BusinessObject.Models.Version> GetByIdAsync(int id)
        {
            return await _versionRepository.GetByIdAsync(id);
        }

        public async Task<List<BusinessObject.Models.Version>> SearchAsync(string keyword)
        {
            return await _versionRepository.SearchAsync(keyword);
        }

        public async Task AddAsync(BusinessObject.Models.Version version)
        {
            await _versionRepository.AddAsync(version);
        }

        public async Task UpdateAsync(BusinessObject.Models.Version version)
        {
            await _versionRepository.UpdateAsync(version);
        }

        public async Task DeleteAsync(int id)
        {
            await _versionRepository.DeleteAsync(id);
        }
    }
}
