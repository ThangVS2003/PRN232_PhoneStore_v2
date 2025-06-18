// Service/IService/IUserService.cs
using BusinessObject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<List<User>> SearchAsync(string username, string email, int? role);
    }
}