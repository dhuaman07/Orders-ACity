using MediatR;
using SOL.MS.Security.Application.DTOs.Orders;
using SOL.MS.Security.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Orders.Queries.GetOrdersByCustomer
{
    public class GetOrdersByCustomerHandler : IRequestHandler<GetOrdersByCustomerQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByCustomerHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetByCustomerNameAsync(request.CustomerName);

            return orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                CustomerName = o.CustomerName,
                Status = o.Status,
                Total = o.Total,
                CreatedAt = o.CreatedAt
            });
        }
    }
}
