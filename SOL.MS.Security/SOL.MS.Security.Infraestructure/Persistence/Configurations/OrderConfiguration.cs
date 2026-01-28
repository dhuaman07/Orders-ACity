using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SOL.MS.Security.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Infraestructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
           
            builder.ToTable("Orders");
           
            builder.HasKey(o => o.OrderId);
     
            builder.Property(o => o.OrderId)
                .HasColumnName("OrderId");

            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("OrderNumber");
          
            builder.HasIndex(o => o.OrderNumber)
                .IsUnique()
                .HasDatabaseName("IX_Orders_OrderNumber");

            builder.Property(o => o.OrderDate)
                .IsRequired()
                .HasColumnName("OrderDate");
            
            builder.HasIndex(o => o.OrderDate)
                .HasDatabaseName("IX_Orders_OrderDate");

            builder.Property(o => o.CustomerName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("CustomerName");
           
            builder.HasIndex(o => o.CustomerName)
                .HasDatabaseName("IX_Orders_CustomerName");

            builder.Property(o => o.CustomerAddress)
                .HasMaxLength(200)
                .HasColumnName("CustomerAddress");

            builder.Property(o => o.CustomerPhone)
                .HasMaxLength(50)
                .HasColumnName("CustomerPhone");

            builder.Property(o => o.CustomerEmail)
                .HasMaxLength(100)
                .HasColumnName("CustomerEmail");

            builder.Property(o => o.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("Status");
           
            builder.HasIndex(o => o.Status)
                .HasDatabaseName("IX_Orders_Status");

            builder.Property(o => o.Subtotal)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("Subtotal");

            builder.Property(o => o.Tax)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("Tax");

            builder.Property(o => o.Total)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("Total");

            builder.Property(o => o.Notes)
                .HasMaxLength(500)
                .HasColumnName("Notes");

            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasColumnName("CreatedAt");

            builder.Property(o => o.UpdatedAt)
                .HasColumnName("UpdatedAt");

           
            builder.HasMany(o => o.OrderDetails)
                .WithOne(d => d.Order)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Metadata
                .FindNavigation(nameof(Order.OrderDetails))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
