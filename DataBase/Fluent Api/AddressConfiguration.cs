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
    public class AddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.HasNoKey();
            builder.Property(a => a.Address)
                .IsRequired()
                .HasMaxLength(400);
            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(40);
            builder.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(30);
            builder.Property(a=>a.PostCode)
                .IsRequired()
                .HasMaxLength(10);
        }
    }
}
