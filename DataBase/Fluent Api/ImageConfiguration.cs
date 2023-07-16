using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DataBase.Fluent_Api
{
    public class ImageConfiguration : IEntityTypeConfiguration<ImagesInformation>
    {
        public void Configure(EntityTypeBuilder<ImagesInformation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(i=>i.imageAddress)
                .IsRequired();
        }
    }
}
