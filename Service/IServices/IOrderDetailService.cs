using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IOrderDetailService
    {
        Task<List<OrderDetail>> GetAllAsync();
        Task<OrderDetail> GetByIdAsync(int id);
        Task AddAsync(OrderDetail orderDetail);
        Task UpdateAsync(OrderDetail orderDetail);
        Task DeleteAsync(int id);
    }
}