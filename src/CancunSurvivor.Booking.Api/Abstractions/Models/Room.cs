using System.Collections.Generic;

namespace CancunSurvivor.Booking.Api.Abstractions.Models
{
    /// <summary>
    /// Represents the room stored in the data store.
    /// </summary>
    public class Room : BaseEntity
    {
        /// <summary>
        /// The name of the <see cref="Room"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of <see cref="Reservation"/> of the <see cref="Room"/>.
        /// </summary>
        public IList<Reservation> Reservations { get; set; }
    }
}
