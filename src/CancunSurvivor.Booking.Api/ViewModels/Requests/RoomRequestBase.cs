using System.ComponentModel.DataAnnotations;
using CancunSurvivor.Booking.Api.Abstractions;

namespace CancunSurvivor.Booking.Api.ViewModels.Requests
{
    /// <summary>
    /// Represents a base request to a room.
    /// </summary>
    public abstract class RoomRequestBase : IEntityRequest
    {
        /// <summary>
        /// The name of the room.
        /// </summary>
        [Required]
        public string Name { get; init; }
    }
}
