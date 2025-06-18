// Service/IService/IAuthService.cs
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(string username, string password);
    }
}