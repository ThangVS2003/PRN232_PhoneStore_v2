// BusinessObject/ViewModels/DashboardViewModel.cs
using System;
using System.Collections.Generic;

namespace BusinessObject.ViewModels
{
    public class DashboardViewModel
    {
        public int UserCount { get; set; }
        public int ProductCount { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalDeposit { get; set; }
        public decimal Profit { get; set; }
        public decimal RevenueBetweenDates { get; set; } // Doanh thu tổng quát
        public List<ProductViewModel> TopSellingProducts { get; set; }

        public DashboardViewModel()
        {
            TopSellingProducts = new List<ProductViewModel>();
        }
    }

    public class ProductViewModel
    {
        public string Name { get; set; }
        public string MainImage { get; set; }
    }
}