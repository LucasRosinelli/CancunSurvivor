using CancunSurvivor.Booking.Api.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CancunSurvivor.Booking.Api.Repositories.Mappings
{
    /// <summary>
    /// The <see cref="Reservation"/> mapping.
    /// </summary>
    public class ReservationMapping : IEntityTypeConfiguration<Reservation>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.CustomerEmail).IsRequired().HasMaxLength(500);
            builder.Property(x => x.CheckInDate).IsRequired();
            builder.Property(x => x.CheckOutDate).IsRequired();
        }
    }
}
