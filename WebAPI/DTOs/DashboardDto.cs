using BusinessObject.Models;

namespace WebAPI.DTOs
{
    public class DashboardDto
    {
        public List<User> Users { get; set; }
        public List<FeedbackProduct> FeedbackProducts { get; set; }
        public List<Comment> Comments { get; set; }
        public List<FeedbackOrder> FeedbackOrders { get; set; }
        public List<Product> Products { get; set; }
        public List<Order> Orders { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal PercentGrowth { get; set; }
        public List<Product> BestSellingProducts { get; set; }
        public List<decimal> MonthlyRevenues { get; set; }

        public DashboardDto()
        {
            Users = new List<User>();
            FeedbackProducts = new List<FeedbackProduct>();
            Comments = new List<Comment>();
            FeedbackOrders = new List<FeedbackOrder>();
            Products = new List<Product>();
            Orders = new List<Order>();
            OrderDetails = new List<OrderDetail>();
            BestSellingProducts = new List<Product>();
            MonthlyRevenues = new List<decimal>(new decimal[12]);
        }
    }
}
