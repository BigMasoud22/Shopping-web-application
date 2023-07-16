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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FullName)
                .IsRequired(false)
                .HasMaxLength(120);

            builder.HasOne(u => u.Address)
        .WithOne(a => a.User)
        .HasForeignKey<UserAddress>(a => a.UserId);
        }
    }
}
