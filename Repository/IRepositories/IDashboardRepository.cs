// Repository/IRepositories/IDashboardRepository.cs
using BusinessObject.Models;
using BusinessObject.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IDashboardRepository
    {
        Task<DashboardViewModel> GetDashboardData(int? topProducts = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> CalculateTotalRevenue(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> CalculateTotalDeposit();
        Task<List<Product>> GetTopSellingProducts(int? top);
    }
}