using BusinessObject.Models;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<List<OrderDetail>> GetAllAsync()
        {
            return await _orderDetailRepository.GetAllAsync();
        }

        public async Task<OrderDetail> GetByIdAsync(int id)
        {
            return await _orderDetailRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            await _orderDetailRepository.AddAsync(orderDetail);
        }

        public async Task UpdateAsync(OrderDetail orderDetail)
        {
            await _orderDetailRepository.UpdateAsync(orderDetail);
        }

        public async Task DeleteAsync(int id)
        {
            await _orderDetailRepository.DeleteAsync(id);
        }
    }
}