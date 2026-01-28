using MediatR;
using SOL.MS.Security.Application.DTOs.Orders;
using SOL.MS.Security.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);

            if (order == null)
                return null;

            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                CustomerName = order.CustomerName,
                CustomerAddress = order.CustomerAddress,
                CustomerPhone = order.CustomerPhone,
                CustomerEmail = order.CustomerEmail,
                Status = order.Status,
                Subtotal = order.Subtotal,
                Tax = order.Tax,
                Total = order.Total,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderDetails = order.OrderDetails?.Select(d => new OrderDetailDto
                {
                    OrderDetailId = d.OrderDetailId,
                    OrderId = d.OrderId,
                    ProductName = d.ProductName,
                    ProductCode = d.ProductCode,
                    Description = d.Description,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Discount = d.Discount,
                    Subtotal = d.Subtotal,
                    CreatedAt = d.CreatedAt
                }).ToList()
            };
        }
    }
}
