using MediatR;
using SOL.MS.Security.Application.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommand : IRequest<OrderDto>
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public List<UpdateOrderDetailCommand> OrderDetails { get; set; }
    }

    public class UpdateOrderDetailCommand
    {
        public int? OrderDetailId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
