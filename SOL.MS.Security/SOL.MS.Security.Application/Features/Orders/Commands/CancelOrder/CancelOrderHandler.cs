using MediatR;
using SOL.MS.Security.Application.DTOs.Orders;
using SOL.MS.Security.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;

        public CancelOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
                throw new ArgumentException($"Pedido con ID {request.OrderId} no encontrado");

            order.Cancel();

            var cancelled = await _orderRepository.UpdateAsync(order);

            return new OrderDto
            {
                OrderId = cancelled.OrderId,
                OrderNumber = cancelled.OrderNumber,
                OrderDate = cancelled.OrderDate,
                CustomerName = cancelled.CustomerName,
                CustomerAddress = cancelled.CustomerAddress,
                CustomerPhone = cancelled.CustomerPhone,
                CustomerEmail = cancelled.CustomerEmail,
                Status = cancelled.Status,
                Subtotal = cancelled.Subtotal,
                Tax = cancelled.Tax,
                Total = cancelled.Total,
                Notes = cancelled.Notes,
                CreatedAt = cancelled.CreatedAt,
                UpdatedAt = cancelled.UpdatedAt,
                OrderDetails = cancelled.OrderDetails?.Select(d => new OrderDetailDto
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
