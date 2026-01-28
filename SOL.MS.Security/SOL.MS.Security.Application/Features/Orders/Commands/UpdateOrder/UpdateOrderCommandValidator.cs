using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("El ID del pedido debe ser mayor a 0");

            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("El nombre del cliente es requerido")
                .MaximumLength(100);

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("El estado es requerido")
                .Must(s => new[] { "Pending", "Processing", "Completed", "Cancelled" }.Contains(s))
                .WithMessage("Estado inválido");

            RuleForEach(x => x.OrderDetails).ChildRules(detail =>
            {
                detail.RuleFor(x => x.ProductName).NotEmpty().MaximumLength(200);
                detail.RuleFor(x => x.Quantity).GreaterThan(0);
                detail.RuleFor(x => x.UnitPrice).GreaterThan(0);
                detail.RuleFor(x => x.Discount).GreaterThanOrEqualTo(0);
            }).When(x => x.OrderDetails != null);
        }
    }
}
