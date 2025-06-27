// Repository/Repositories/DashboardRepository.cs
using BusinessObject.Models;
using BusinessObject.ViewModels;
using Microsoft.EntityFrameworkCore;
using Repository.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DbPhoneStoreContext _context;

        public DashboardRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardData(int? topProducts = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            topProducts = topProducts ?? 5; // Mặc định 5 nếu null
            var model = new DashboardViewModel
            {
                UserCount = await _context.Users.CountAsync(),
                ProductCount = await _context.Products.CountAsync(),
                OrderCount = await _context.Orders.CountAsync(),
                TotalRevenue = await CalculateTotalRevenue(),
                TotalDeposit = await CalculateTotalDeposit(),
                Profit = await CalculateTotalRevenue() - await CalculateTotalDeposit(), // Tiền lãi = Doanh thu - Tiền nhập
                RevenueBetweenDates = await CalculateTotalRevenue(),
                TopSellingProducts = (await GetTopSellingProducts(topProducts.Value))
                    .Select(p => new ProductViewModel
                    {
                        Name = p.Name,
                        MainImage = p.MainImage ?? "/img/default.png"
                    }).ToList()
            };
            return model;
        }

        public async Task<decimal> CalculateTotalRevenue(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _context.OrderDetails
                .Join(_context.Orders, od => od.OrderId, o => o.Id, (od, o) => new { od, o })
                .Where(x => (!startDate.HasValue || x.o.OrderDate >= startDate) && (!endDate.HasValue || x.o.OrderDate <= endDate))
                .SumAsync(x => x.od.UnitPrice * x.od.Quantity);
        }

        public async Task<decimal> CalculateTotalDeposit()
        {
            return await _context.ProductVariants.SumAsync(pv => pv.OriginalPrice * pv.StockQuantity);
        }

        public async Task<List<Product>> GetTopSellingProducts(int? top)
        {
            return await _context.OrderDetails
                .GroupBy(od => od.ProductVariant.ProductId)
                .Select(g => new Product
                {
                    Id = g.Key,
                    Name = g.First().ProductVariant.Product.Name,
                    MainImage = g.First().ProductVariant.Product.MainImage
                })
                .Take(top ?? 5)
                .ToListAsync();
        }
    }
}