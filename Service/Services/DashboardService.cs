// Service/Services/DashboardService.cs
using BusinessObject.ViewModels;
using Repository.IRepositories;
using Service.IServices;
using System.Threading.Tasks;

namespace Service.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardViewModel> GetDashboardData(int? topProducts = null)
        {
            return await _dashboardRepository.GetDashboardData(topProducts);
        }

        public async Task<decimal> GetProfit(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _dashboardRepository.CalculateTotalRevenue(startDate, endDate) -
                   await _dashboardRepository.CalculateTotalDeposit();
        }
    }
}