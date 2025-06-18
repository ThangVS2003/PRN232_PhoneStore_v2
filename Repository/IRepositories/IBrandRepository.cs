// Repository/IRepository/IBrandRepository.cs
using BusinessObject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IBrandRepository
    {
        Task<List<Brand>> GetAllAsync();
        Task<Brand> GetByIdAsync(int id);
        Task<List<Brand>> SearchAsync(string name);
    }
}