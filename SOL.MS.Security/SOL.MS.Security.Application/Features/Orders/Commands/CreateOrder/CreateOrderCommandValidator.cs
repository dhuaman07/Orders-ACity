using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("El nombre del cliente es requerido")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

            RuleFor(x => x.CustomerAddress)
                .MaximumLength(200).WithMessage("La dirección no puede exceder 200 caracteres");

            RuleFor(x => x.CustomerPhone)
                .MaximumLength(50).WithMessage("El teléfono no puede exceder 50 caracteres");

            RuleFor(x => x.CustomerEmail)
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.CustomerEmail));

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Las notas no pueden exceder 500 caracteres");

            RuleFor(x => x.OrderDetails)
                .NotEmpty().WithMessage("Debe agregar al menos un detalle al pedido");

            RuleForEach(x => x.OrderDetails).ChildRules(detail =>
            {
                detail.RuleFor(x => x.ProductName)
                    .NotEmpty().WithMessage("El nombre del producto es requerido")
                    .MaximumLength(200);

                detail.RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");

                detail.RuleFor(x => x.UnitPrice)
                    .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

                detail.RuleFor(x => x.Discount)
                    .GreaterThanOrEqualTo(0).WithMessage("El descuento no puede ser negativo");
            });
        }
    }
}
