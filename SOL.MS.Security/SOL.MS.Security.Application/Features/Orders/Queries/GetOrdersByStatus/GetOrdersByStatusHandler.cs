using MediatR;
using SOL.MS.Security.Application.DTOs.Orders;
using SOL.MS.Security.Domain.Entities;
using SOL.MS.Security.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Orders.Queries.GetOrdersByStatus
{
    public class GetOrdersByStatusHandler : IRequestHandler<GetOrdersByStatusQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByStatusHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
        {
            if (!OrderStatus.IsValid(request.Status))
                throw new ArgumentException($"Estado inválido: {request.Status}");

            var orders = await _orderRepository.GetByStatusAsync(request.Status);

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
