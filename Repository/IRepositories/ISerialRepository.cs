using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ISerialRepository
    {
        Task<List<Serial>> GetAllAsync();
        Task<Serial?> GetByIdAsync(int id);
        Task<List<Serial>> SearchAsync(string keyword);
        Task AddAsync(Serial serial);
        Task UpdateAsync(Serial serial);
        Task DeleteAsync(int id);
    }
}
