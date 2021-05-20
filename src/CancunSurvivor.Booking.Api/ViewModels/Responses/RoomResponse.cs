using System;
using CancunSurvivor.Booking.Api.Abstractions;

#nullable disable warnings
namespace CancunSurvivor.Booking.Api.ViewModels.Responses
{
    /// <summary>
    /// Represents a room response.
    /// </summary>
    public class RoomResponse : IEntityResponse
    {
        /// <summary>
        /// The id of the room.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// The name of the room.
        /// </summary>
        public string Name { get; init; }
    }
}
#nullable enable warnings
