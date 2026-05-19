using ApiMiniPrj.Domain.Models.Organizers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiMiniPrj.Persistence.Configurations
{
    internal class OrganizerConfiguration : IEntityTypeConfiguration<Organizer>
    {
        public void Configure(EntityTypeBuilder<Organizer> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.FullName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(o => o.Email)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(o => o.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(o => o.LogoUrl)
                .HasMaxLength(200);
            

        }
    }
}
