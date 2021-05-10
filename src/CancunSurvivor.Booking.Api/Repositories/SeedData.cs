using System.Linq;
using CancunSurvivor.Booking.Api.Abstractions.Models;

namespace CancunSurvivor.Booking.Api.Repositories
{
    public static class SeedData
    {
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
