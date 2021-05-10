using CancunSurvivor.Booking.Api.Abstractions.Models;
using CancunSurvivor.Booking.Api.Repositories.Mappings;
using Microsoft.EntityFrameworkCore;

namespace CancunSurvivor.Booking.Api.Repositories
{
    /// <summary>
    /// Represents the data store of the application.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The context options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Rooms = Set<Room>();
            Reservations = Set<Reservation>();
        }

        /// <summary>
        /// The <see cref="DbSet{TEntity}"/> of <see cref="Room"/>.
        /// </summary>
        public DbSet<Room> Rooms { get; set; }

        /// <summary>
        /// The <see cref="DbSet{TEntity}"/> of <see cref="Reservation"/>.
        /// </summary>
        public DbSet<Reservation> Reservations { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RoomMapping());
            modelBuilder.ApplyConfiguration(new ReservationMapping());

            base.OnModelCreating(modelBuilder);
        }
    }
}
