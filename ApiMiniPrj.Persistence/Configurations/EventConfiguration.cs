using ApiMiniPrj.Domain.Models.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMiniPrj.Persistence.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength (200);
            builder.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(e => e.Date)
                .IsRequired();
            builder.Property(e => e.BannerImageUrl)
                .IsRequired(false)
                .HasMaxLength(500);
            builder.HasOne(e => e.Organizer)
               .WithMany(o => o.Events)
               .HasForeignKey(e => e.OrganizerId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
