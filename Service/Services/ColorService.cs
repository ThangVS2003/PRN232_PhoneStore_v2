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
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;

        public ColorService(IColorRepository colorRepository)
        {
            _colorRepository = colorRepository;
        }

        public async Task<List<Color>> GetAllAsync()
        {
            return await _colorRepository.GetAllAsync();
        }

        public async Task<Color> GetByIdAsync(int id)
        {
            return await _colorRepository.GetByIdAsync(id);
        }

        public async Task<List<Color>> SearchAsync(string keyword)
        {
            return await _colorRepository.SearchAsync(keyword);
        }

        public async Task AddAsync(Color color)
        {
            await _colorRepository.AddAsync(color);
        }

        public async Task UpdateAsync(Color color)
        {
            await _colorRepository.UpdateAsync(color);
        }

        public async Task DeleteAsync(int id)
        {
            await _colorRepository.DeleteAsync(id);
        }
    }

}
