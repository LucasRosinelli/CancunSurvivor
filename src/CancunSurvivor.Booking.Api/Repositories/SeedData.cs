using System.Linq;
using CancunSurvivor.Booking.Api.Abstractions.Models;

namespace CancunSurvivor.Booking.Api.Repositories
{
    /// <summary>
    /// Represents the data to seed.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Initializes the <see cref="AppDbContext"/> with default data.
        /// </summary>
        /// <param name="dbContext">The <see cref="AppDbContext"/>.</param>
        public static void Initialize(AppDbContext dbContext)
        {
            if (dbContext.Rooms.Any())
            {
                return;
            }

            var veryLastHotelRoom = new Room
            {
                Id = AppConstants.DefaultRoomId,
                Name = "Very last hotel in Cancun with its only one room available",
            };
            dbContext.Rooms.Add(veryLastHotelRoom);

            dbContext.SaveChanges();
        }
    }
}
