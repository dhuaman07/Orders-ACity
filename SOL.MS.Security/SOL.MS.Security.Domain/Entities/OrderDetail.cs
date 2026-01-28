using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Domain.Entities
{
    public class OrderDetail
    {
        public int OrderDetailId { get; private set; }
        public int OrderId { get; private set; }
        public string ProductName { get; private set; }
        public string ProductCode { get; private set; }
        public string Description { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Discount { get; private set; }
        public decimal Subtotal { get; private set; }
        public DateTime CreatedAt { get; private set; }

     
        public Order Order { get; private set; }

 
        private OrderDetail()
        {
        }

   
        internal static OrderDetail Create(
            int orderId,
            string productName,
            string productCode,
            string description,
            int quantity,
            decimal unitPrice,
            decimal discount)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name is required", nameof(productName));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            if (discount < 0)
                throw new ArgumentException("Discount cannot be negative", nameof(discount));

            var detail = new OrderDetail
            {
                OrderId = orderId,
                ProductName = productName,
                ProductCode = productCode,
                Description = description,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Discount = discount,
                CreatedAt = DateTime.UtcNow
            };

            detail.CalculateSubtotal();
            return detail;
        }

    

        internal void Update(
            string productName,
            string productCode,
            string description,
            int quantity,
            decimal unitPrice,
            decimal discount)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name is required", nameof(productName));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            if (discount < 0)
                throw new ArgumentException("Discount cannot be negative", nameof(discount));

            ProductName = productName;
            ProductCode = productCode;
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Discount = discount;

            CalculateSubtotal();
        }

        internal void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            Quantity = quantity;
            CalculateSubtotal();
        }

        internal void UpdatePrice(decimal unitPrice)
        {
            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            UnitPrice = unitPrice;
            CalculateSubtotal();
        }

        internal void ApplyDiscount(decimal discount)
        {
            if (discount < 0)
                throw new ArgumentException("Discount cannot be negative", nameof(discount));

            var maxDiscount = Quantity * UnitPrice;
            if (discount > maxDiscount)
                throw new ArgumentException($"Discount cannot exceed total price ({maxDiscount:C})", nameof(discount));

            Discount = discount;
            CalculateSubtotal();
        }

        private void CalculateSubtotal()
        {
            Subtotal = (Quantity * UnitPrice) - Discount;

            if (Subtotal < 0)
                throw new InvalidOperationException("Subtotal cannot be negative");
        }

      
        internal void SetOrderDetailId(int orderDetailId)
        {
            OrderDetailId = orderDetailId;
        }
    }
}
