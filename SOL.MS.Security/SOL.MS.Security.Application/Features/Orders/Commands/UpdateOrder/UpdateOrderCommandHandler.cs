using MediatR;
using SOL.MS.Security.Application.DTOs.Orders;
using SOL.MS.Security.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;

        public UpdateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
                throw new ArgumentException($"Pedido con ID {request.OrderId} no encontrado");

            order.UpdateCustomerInfo(
                request.CustomerName,
                request.CustomerAddress,
                request.CustomerPhone,
                request.CustomerEmail);

            if (request.Notes != order.Notes)
                order.UpdateNotes(request.Notes);

            if (request.Status != order.Status)
                order.ChangeStatus(request.Status);

            if (request.OrderDetails != null)
            {
                var existingIds = order.OrderDetails.Select(d => d.OrderDetailId).ToList();
                var updatedIds = request.OrderDetails
                    .Where(d => d.OrderDetailId.HasValue)
                    .Select(d => d.OrderDetailId.Value)
                    .ToList();

                foreach (var id in existingIds.Where(id => !updatedIds.Contains(id)))
                {
                    order.RemoveDetail(id);
                }

                foreach (var detail in request.OrderDetails)
                {
                    if (detail.OrderDetailId.HasValue && detail.OrderDetailId.Value > 0)
                    {
                        order.UpdateDetail(
                            detail.OrderDetailId.Value,
                            detail.ProductName,
                            detail.ProductCode,
                            detail.Description,
                            detail.Quantity,
                            detail.UnitPrice,
                            detail.Discount);
                    }
                    else
                    {
                        order.AddDetail(
                            detail.ProductName,
                            detail.ProductCode,
                            detail.Description,
                            detail.Quantity,
                            detail.UnitPrice,
                            detail.Discount);
                    }
                }
            }

            var updated = await _orderRepository.UpdateAsync(order);

            return new OrderDto
            {
                OrderId = updated.OrderId,
                OrderNumber = updated.OrderNumber,
                OrderDate = updated.OrderDate,
                CustomerName = updated.CustomerName,
                CustomerAddress = updated.CustomerAddress,
                CustomerPhone = updated.CustomerPhone,
                CustomerEmail = updated.CustomerEmail,
                Status = updated.Status,
                Subtotal = updated.Subtotal,
                Tax = updated.Tax,
                Total = updated.Total,
                Notes = updated.Notes,
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt,
                OrderDetails = updated.OrderDetails?.Select(d => new OrderDetailDto
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
