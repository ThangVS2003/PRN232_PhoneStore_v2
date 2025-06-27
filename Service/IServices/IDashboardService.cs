// Service/IServices/IDashboardService.cs
using BusinessObject.ViewModels;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardData(int? topProducts = null);
        Task<decimal> GetProfit(DateTime? startDate = null, DateTime? endDate = null);
    }
}