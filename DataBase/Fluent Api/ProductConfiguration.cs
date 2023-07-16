using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DataBase.Fluent_Api
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.id);
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(p => p.Price)
                   .IsRequired();
            builder.Property(p => p.Description)
                   .HasMaxLength(500);
            builder.Property(p=>p.IsAvailable)
                .IsRequired();
        }
    }
}
