using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Fluent_Api
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.OrderDate)
                   .IsRequired();
            builder.Property(o => o.OrderTotal)
                .IsRequired()
                .HasColumnType("decimal");
            builder.Property(o=>o.ShippingDate)
                .IsRequired();
        }
    }
}
