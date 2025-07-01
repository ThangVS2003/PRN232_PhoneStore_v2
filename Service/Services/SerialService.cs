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
    public class SerialService : ISerialService
    {
        private readonly ISerialRepository _serialRepository;

        public SerialService(ISerialRepository serialRepository)
        {
            _serialRepository = serialRepository;
        }

        public async Task<List<Serial>> GetAllAsync()
        {
            return await _serialRepository.GetAllAsync();
        }

        public async Task<Serial> GetByIdAsync(int id)
        {
            return await _serialRepository.GetByIdAsync(id);
        }

        public async Task<List<Serial>> SearchAsync(string keyword)
        {
            return await _serialRepository.SearchAsync(keyword);
        }

        public async Task<List<Serial>> SearchByProductVariantIdAsync(string keyword, int? productVariantId)
        {
            return await _serialRepository.SearchByProductVariantIdAsync(keyword, productVariantId);
        }

        public async Task AddAsync(Serial serial)
        {
            await _serialRepository.AddAsync(serial);
        }

        public async Task UpdateAsync(Serial serial)
        {
            await _serialRepository.UpdateAsync(serial);
        }

        public async Task DeleteAsync(int id)
        {
            await _serialRepository.DeleteAsync(id);
        }

        public async Task<List<Serial>> GetByProductVariantIdAsync(int productVariantId)
        {
            return await _serialRepository.GetByProductVariantIdAsync(productVariantId);
        }
    }
}
