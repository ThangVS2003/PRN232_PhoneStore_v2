using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repository.IRepository
{
    public interface IColorRepository
    {
        Task<List<Color>> GetAllAsync();
        Task<Color?> GetByIdAsync(int id);
        Task<List<Color>> SearchAsync(string keyword);
        Task AddAsync(Color color);
        Task UpdateAsync(Color color);
        Task DeleteAsync(int id);
    }

}
