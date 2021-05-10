using CancunSurvivor.Booking.Api.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CancunSurvivor.Booking.Api.Repositories.Mappings
{
    /// <summary>
    /// The <see cref="Room"/> mapping.
    /// </summary>
    public class RoomMapping : IEntityTypeConfiguration<Room>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            builder.HasMany(x => x.Reservations).WithOne(x => x.Room).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
