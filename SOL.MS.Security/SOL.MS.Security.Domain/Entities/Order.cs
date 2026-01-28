using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; private set; }
        public string OrderNumber { get; private set; }
        public DateTime OrderDate { get; private set; }
        public string CustomerName { get; private set; }
        public string CustomerAddress { get; private set; }
        public string CustomerPhone { get; private set; }
        public string CustomerEmail { get; private set; }
        public string Status { get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal Tax { get; private set; }
        public decimal Total { get; private set; }
        public string Notes { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        
        private readonly List<OrderDetail> _orderDetails = new List<OrderDetail>();

        
        public IReadOnlyCollection<OrderDetail> OrderDetails => _orderDetails;

       
        private Order()
        {
           
        }


        public static Order Create(
            string orderNumber,
            string customerName,
            string customerAddress,
            string customerPhone,
            string customerEmail,
            string notes)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                throw new ArgumentException("Order number is required", nameof(orderNumber));

            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name is required", nameof(customerName));

            return new Order
            {
                OrderNumber = orderNumber,
                OrderDate = DateTime.UtcNow,
                CustomerName = customerName,
                CustomerAddress = customerAddress,
                CustomerPhone = customerPhone,
                CustomerEmail = customerEmail?.ToLowerInvariant(),
                Status = OrderStatus.Pending,
                Notes = notes,
                Subtotal = 0,
                Tax = 0,
                Total = 0,
                CreatedAt = DateTime.UtcNow
               
            };
        }


        public OrderDetail AddDetail(
            string productName,
            string productCode,
            string description,
            int quantity,
            decimal unitPrice,
            decimal discount = 0)
        {
            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot add details to a cancelled order");

            if (Status == OrderStatus.Completed)
                throw new InvalidOperationException("Cannot add details to a completed order");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            if (discount < 0)
                throw new ArgumentException("Discount cannot be negative", nameof(discount));

            var detail = OrderDetail.Create(
                OrderId,
                productName,
                productCode,
                description,
                quantity,
                unitPrice,
                discount);

            _orderDetails.Add(detail);
            RecalculateTotals();

            return detail;
        }

        public void RemoveDetail(int orderDetailId)
        {
            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot remove details from a cancelled order");

            if (Status == OrderStatus.Completed)
                throw new InvalidOperationException("Cannot remove details from a completed order");

            var detail = _orderDetails.FirstOrDefault(d => d.OrderDetailId == orderDetailId);
            if (detail == null)
                throw new ArgumentException($"Detail with ID {orderDetailId} not found");

            _orderDetails.Remove(detail);
            RecalculateTotals();
        }

        public void UpdateDetail(
            int orderDetailId,
            string productName,
            string productCode,
            string description,
            int quantity,
            decimal unitPrice,
            decimal discount)
        {
            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot update details of a cancelled order");

            if (Status == OrderStatus.Completed)
                throw new InvalidOperationException("Cannot update details of a completed order");

            var detail = _orderDetails.FirstOrDefault(d => d.OrderDetailId == orderDetailId);
            if (detail == null)
                throw new ArgumentException($"Detail with ID {orderDetailId} not found");

            detail.Update(productName, productCode, description, quantity, unitPrice, discount);
            RecalculateTotals();
        }

        public void UpdateCustomerInfo(
            string customerName,
            string customerAddress,
            string customerPhone,
            string customerEmail)
        {
            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot update customer info of a cancelled order");

            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name is required", nameof(customerName));

            CustomerName = customerName;
            CustomerAddress = customerAddress;
            CustomerPhone = customerPhone;
            CustomerEmail = customerEmail?.ToLowerInvariant();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeStatus(string newStatus)
        {
            if (!OrderStatus.IsValid(newStatus))
                throw new ArgumentException($"Invalid status: {newStatus}");

       
            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status of a cancelled order");

            if (Status == OrderStatus.Completed && newStatus != OrderStatus.Cancelled)
                throw new InvalidOperationException("Can only cancel a completed order");

            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }

        public void StartProcessing()
        {
            if (!_orderDetails.Any())
                throw new InvalidOperationException("Cannot process an order without details");

            ChangeStatus(OrderStatus.Processing);
        }

        public void Complete()
        {
            if (!_orderDetails.Any())
                throw new InvalidOperationException("Cannot complete an order without details");

            if (Status != OrderStatus.Processing)
                throw new InvalidOperationException("Only orders in Processing status can be completed");

            ChangeStatus(OrderStatus.Completed);
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Order is already cancelled");

            ChangeStatus(OrderStatus.Cancelled);
        }

        private void RecalculateTotals()
        {
            Subtotal = _orderDetails.Sum(d => d.Subtotal);
            Tax = Subtotal * 0.18m; 
            Total = Subtotal + Tax;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public static class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";

        public static bool IsValid(string status)
        {
            return status == Pending ||
                   status == Processing ||
                   status == Completed ||
                   status == Cancelled;
        }

        public static IEnumerable<string> GetAll()
        {
            yield return Pending;
            yield return Processing;
            yield return Completed;
            yield return Cancelled;
        }
    }
}
