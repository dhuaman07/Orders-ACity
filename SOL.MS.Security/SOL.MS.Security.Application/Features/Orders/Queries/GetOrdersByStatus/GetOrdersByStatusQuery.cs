using MediatR;
using SOL.MS.Security.Application.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Orders.Queries.GetOrdersByStatus
{
    public class GetOrdersByStatusQuery : IRequest<IEnumerable<OrderDto>>
    {
        public string Status { get; set; }

        public GetOrdersByStatusQuery(string status)
        {
            Status = status;
        }
    }
}
