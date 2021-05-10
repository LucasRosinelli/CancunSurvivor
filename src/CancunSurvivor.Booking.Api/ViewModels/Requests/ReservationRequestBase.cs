using System;
using System.ComponentModel.DataAnnotations;
using CancunSurvivor.Booking.Api.Abstractions;

namespace CancunSurvivor.Booking.Api.ViewModels.Requests
{
    /// <summary>
    /// Represents a base request to a reservation.
    /// </summary>
    public abstract class ReservationRequestBase : IEntityRequest
    {
        /// <summary>
        /// The check-in date of the reservation.
        /// </summary>
        [Required]
        public DateTime CheckInDate { get; init; }

        /// <summary>
        /// The check-out date of the reservation.
        /// </summary>
        [Required]
        public DateTime CheckOutDate { get; init; }

        /// <summary>
        /// The room id of the reservation.
        /// </summary>
        internal Guid RoomId
        {
            get => AppConstants.DefaultRoomId;
        }
    }
}
