using MediatR;
using SOL.MS.Security.Application.DTOs.Orders;
using SOL.MS.Security.Domain.Entities;
using SOL.MS.Security.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderNumber = await _orderRepository.GenerateOrderNumberAsync();

            var order = Order.Create(
                orderNumber,
                request.CustomerName,
                request.CustomerAddress,
                request.CustomerPhone,
                request.CustomerEmail,
                request.Notes);

            foreach (var detail in request.OrderDetails)
            {
                order.AddDetail(
                    detail.ProductName,
                    detail.ProductCode,
                    detail.Description,
                    detail.Quantity,
                    detail.UnitPrice,
                    detail.Discount);
            }

            var createdOrder = await _orderRepository.CreateAsync(order);

            return new OrderDto
            {
                OrderId = createdOrder.OrderId,
                OrderNumber = createdOrder.OrderNumber,
                OrderDate = createdOrder.OrderDate,
                CustomerName = createdOrder.CustomerName,
                CustomerAddress = createdOrder.CustomerAddress,
                CustomerPhone = createdOrder.CustomerPhone,
                CustomerEmail = createdOrder.CustomerEmail,
                Status = createdOrder.Status,
                Subtotal = createdOrder.Subtotal,
                Tax = createdOrder.Tax,
                Total = createdOrder.Total,
                Notes = createdOrder.Notes,
                CreatedAt = createdOrder.CreatedAt,
                UpdatedAt = createdOrder.UpdatedAt,
                OrderDetails = createdOrder.OrderDetails?.Select(d => new OrderDetailDto
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
