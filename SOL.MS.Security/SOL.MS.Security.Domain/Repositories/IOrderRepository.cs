using SOL.MS.Security.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Domain.Repositories
{        
    public interface IOrderRepository
    {           
        Task<Order> GetByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByStatusAsync(string status);
        Task<IEnumerable<Order>> GetByCustomerNameAsync(string customerName);
        Task<Order> GetByOrderNumberAsync(string orderNumber);
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task<bool> DeleteAsync(int orderId);
        Task<bool> ExistsAsync(int orderId);
        Task<string> GenerateOrderNumberAsync();
    }
}
