using System;

#nullable disable warnings
namespace CancunSurvivor.Booking.Api.Abstractions.Models
{
    /// <summary>
    /// Represents the reservation stored in the data store.
    /// </summary>
    public class Reservation : BaseEntity
    {
        /// <summary>
        /// The customer email of the <see cref="Reservation"/>.
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// The check-in date of the <see cref="Reservation"/>.
        /// </summary>
        public DateTime CheckInDate { get; init; }

        /// <summary>
        /// The check-out date of the <see cref="Reservation"/>.
        /// </summary>
        public DateTime CheckOutDate { get; init; }

        /// <summary>
        /// The <see cref="Room"/> id of the <see cref="Reservation"/>.
        /// </summary>
        public Guid RoomId { get; init; }

        /// <summary>
        /// The <see cref="Room"/> which the <see cref="Reservation"/> belongs to.
        /// </summary>
        public Room Room { get; init; }
    }
}
#nullable enable warnings
