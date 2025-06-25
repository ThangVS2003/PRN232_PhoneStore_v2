using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Service.IService
{
    public interface IVersionService
    {
        Task<List<BusinessObject.Models.Version>> GetAllAsync();
        Task<BusinessObject.Models.Version> GetByIdAsync(int id);
        Task<List<BusinessObject.Models.Version>> SearchAsync(string keyword);
        Task AddAsync(BusinessObject.Models.Version version);
        Task UpdateAsync(BusinessObject.Models.Version version);
        Task DeleteAsync(int id);
    }
}
