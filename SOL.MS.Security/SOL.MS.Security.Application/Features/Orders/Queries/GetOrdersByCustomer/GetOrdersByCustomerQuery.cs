using MediatR;
using SOL.MS.Security.Application.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Orders.Queries.GetOrdersByCustomer
{
    public class GetOrdersByCustomerQuery : IRequest<IEnumerable<OrderDto>>
    {
        public string CustomerName { get; set; }

        public GetOrdersByCustomerQuery(string customerName)
        {
            CustomerName = customerName;
        }
    }
}
