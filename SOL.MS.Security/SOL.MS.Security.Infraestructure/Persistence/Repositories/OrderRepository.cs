using Microsoft.EntityFrameworkCore;
using SOL.MS.Security.Domain.Entities;
using SOL.MS.Security.Domain.Repositories;

namespace SOL.MS.Security.Infraestructure.Persistence.Repositories
{

    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task<Order> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

      
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

       
        public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

       
        public async Task<IEnumerable<Order>> GetByCustomerNameAsync(string customerName)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.CustomerName.Contains(customerName))
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        
        public async Task<Order> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        
        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

      
        public async Task<Order> UpdateAsync(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;           

            await _context.SaveChangesAsync();
            return order;
        }
       
        public async Task<bool> DeleteAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> ExistsAsync(int orderId)
        {
            return await _context.Orders.AnyAsync(o => o.OrderId == orderId);
        }
       
        public async Task<string> GenerateOrderNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
           
            var lastOrder = await _context.Orders
                .Where(o => o.OrderNumber.StartsWith($"ORD-{year}"))
                .OrderByDescending(o => o.OrderNumber)
                .FirstOrDefaultAsync();

            if (lastOrder == null)
            {                
                return $"ORD-{year}-0001";
            }
           
            var lastNumberPart = lastOrder.OrderNumber.Split('-').Last();
            var lastNumber = int.Parse(lastNumberPart);
            var newNumber = lastNumber + 1;
           
            return $"ORD-{year}-{newNumber:D4}";
        }
    }
}
