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
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
           
            builder.ToTable("OrderDetails");

          
            builder.HasKey(d => d.OrderDetailId);

          
            builder.Property(d => d.OrderDetailId)
                .HasColumnName("OrderDetailId");

            builder.Property(d => d.OrderId)
                .IsRequired()
                .HasColumnName("OrderId");

           
            builder.HasIndex(d => d.OrderId)
                .HasDatabaseName("IX_OrderDetails_OrderId");

            builder.Property(d => d.ProductName)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("ProductName");

           
            builder.HasIndex(d => d.ProductName)
                .HasDatabaseName("IX_OrderDetails_ProductName");

            builder.Property(d => d.ProductCode)
                .HasMaxLength(100)
                .HasColumnName("ProductCode");

           
            builder.HasIndex(d => d.ProductCode)
                .HasDatabaseName("IX_OrderDetails_ProductCode");

            builder.Property(d => d.Description)
                .HasMaxLength(500)
                .HasColumnName("Description");

            builder.Property(d => d.Quantity)
                .IsRequired()
                .HasColumnName("Quantity");

            builder.Property(d => d.UnitPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasColumnName("UnitPrice");

            builder.Property(d => d.Discount)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("Discount");

            builder.Property(d => d.Subtotal)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("Subtotal");

            builder.Property(d => d.CreatedAt)
                .IsRequired()
                .HasColumnName("CreatedAt");

          
            builder.HasOne(d => d.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
