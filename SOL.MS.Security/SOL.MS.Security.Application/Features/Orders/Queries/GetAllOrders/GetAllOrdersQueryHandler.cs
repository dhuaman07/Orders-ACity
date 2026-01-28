using MediatR;
using SOL.MS.Security.Application.DTOs.Orders;
using SOL.MS.Security.Application.Features.Orders.Queries.GetOrderById;
using SOL.MS.Security.Domain.Repositories;

namespace SOL.MS.Security.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetAllOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetAllAsync();

            return orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                CustomerName = o.CustomerName,
                CustomerAddress = o.CustomerAddress,
                CustomerPhone = o.CustomerPhone,
                CustomerEmail = o.CustomerEmail,
                Status = o.Status,
                Subtotal = o.Subtotal,
                Tax = o.Tax,
                Total = o.Total,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                OrderDetails = o.OrderDetails?.Select(d => new OrderDetailDto
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
            });
        }
    }
}
