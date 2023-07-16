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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.id);
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(c => c.Description)
                .HasMaxLength(200);
        }
    }
}
