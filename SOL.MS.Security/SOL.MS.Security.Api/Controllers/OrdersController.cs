using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SOL.MS.Security.Application.Common;
using SOL.MS.Security.Application.Features.Orders.Commands.CancelOrder;
using SOL.MS.Security.Application.Features.Orders.Commands.CreateOrder;
using SOL.MS.Security.Application.Features.Orders.Commands.DeleteOrder;
using SOL.MS.Security.Application.Features.Orders.Commands.UpdateOrder;
using SOL.MS.Security.Application.Features.Orders.Queries.GetAllOrders;
using SOL.MS.Security.Application.Features.Orders.Queries.GetOrderById;
using SOL.MS.Security.Application.Features.Orders.Queries.GetOrdersByCustomer;
using SOL.MS.Security.Application.Features.Orders.Queries.GetOrdersByStatus;

namespace SOL.MS.Security.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }
     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = new GetAllOrdersQuery();
                var orders = await _mediator.Send(query);
                var result = Result.Success(orders, "Pedidos obtenidos exitosamente");
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = Result.Failure<object>(ex.Message, "Error al obtener los pedidos");
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
     
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var query = new GetOrderByIdQuery(id);
                var order = await _mediator.Send(query);

                if (order == null)
                {
                    var notFoundResult = Result.NotFound<object>("Pedido no encontrado");
                    return NotFound(notFoundResult);
                }

                var result = Result.Success(order, "Pedido obtenido exitosamente");
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = Result.Failure<object>(ex.Message, "Error al obtener el pedido");
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
       
        [HttpGet("by-status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByStatus(string status)
        {
            try
            {
                var query = new GetOrdersByStatusQuery(status);
                var orders = await _mediator.Send(query);
                var result = Result.Success(orders, $"Pedidos con estado '{status}' obtenidos exitosamente");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                var result = Result.Failure<object>(ex.Message, "Estado inválido");
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                var result = Result.Failure<object>(ex.Message, "Error al obtener los pedidos");
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
     
        [HttpGet("by-customer/{customerName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCustomerName(string customerName)
        {
            try
            {
                var query = new GetOrdersByCustomerQuery(customerName);
                var orders = await _mediator.Send(query);
                var result = Result.Success(orders, $"Pedidos del cliente '{customerName}' obtenidos exitosamente");
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = Result.Failure<object>(ex.Message, "Error al obtener los pedidos");
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
       
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            try
            {
                var order = await _mediator.Send(command);
                var result = Result.Success(order, "Pedido creado exitosamente");

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = order.OrderId },
                    result);
            }
            catch (ArgumentException ex)
            {
                var result = Result.Failure<object>(ex.Message, "Datos inválidos");
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                var result = Result.Failure<object>(ex.Message, "Error al crear el pedido");
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
     
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderCommand command)
        {
            try
            {
                if (id != command.OrderId)
                {
                    var badResult = Result.Failure<object>("El ID de la URL no coincide con el ID del pedido", "Solicitud inválida");
                    return BadRequest(badResult);
                }

                var order = await _mediator.Send(command);
                var result = Result.Success(order, "Pedido actualizado exitosamente");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                var result = Result.NotFound<object>(ex.Message);
                return NotFound(result);
            }
            catch (InvalidOperationException ex)
            {
                var result = Result.Failure<object>(ex.Message, "Operación inválida");
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                var result = Result.Failure<object>(ex.Message, "Error al actualizar el pedido");
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
      
        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var command = new CancelOrderCommand(id);
                var order = await _mediator.Send(command);
                var result = Result.Success(order, "Pedido cancelado exitosamente");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                var result = Result.NotFound<object>(ex.Message);
                return NotFound(result);
            }
            catch (InvalidOperationException ex)
            {
                var result = Result.Failure<object>(ex.Message, "No se puede cancelar el pedido");
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                var result = Result.Failure<object>(ex.Message, "Error al cancelar el pedido");
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
     
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteOrderCommand(id);
                var deleted = await _mediator.Send(command);

                if (!deleted)
                {
                    var notFoundResult = Result.NotFound<object>("Pedido no encontrado");
                    return NotFound(notFoundResult);
                }

                var result = Result.Success("Pedido eliminado exitosamente");
                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = Result.Failure<object>(ex.Message, "Error al eliminar el pedido");
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
    }
}
