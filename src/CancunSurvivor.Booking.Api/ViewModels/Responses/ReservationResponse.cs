using System;
using CancunSurvivor.Booking.Api.Abstractions;

#nullable disable warnings
namespace CancunSurvivor.Booking.Api.ViewModels.Responses
{
    /// <summary>
    /// Represents a reservation response.
    /// </summary>
    public class ReservationResponse : IEntityResponse
    {
        /// <summary>
        /// The id of the reservation.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// The customer email of the reservation.
        /// </summary>
        public string CustomerEmail { get; init; }

        /// <summary>
        /// The check-in date of the reservation.
        /// </summary>
        public DateTime CheckInDate { get; init; }

        /// <summary>
        /// The check-out date of the reservation.
        /// </summary>
        public DateTime CheckOutDate { get; init; }

        /// <summary>
        /// The room id of the reservation.
        /// </summary>
        public Guid RoomId { get; init; }
    }
}
#nullable enable warnings
