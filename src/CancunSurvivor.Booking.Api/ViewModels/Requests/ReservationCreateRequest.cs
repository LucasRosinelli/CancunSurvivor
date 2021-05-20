using System.ComponentModel.DataAnnotations;

#nullable disable warnings
namespace CancunSurvivor.Booking.Api.ViewModels.Requests
{
    /// <summary>
    /// Represents a request to create a reservation.
    /// </summary>
    public class ReservationCreateRequest : ReservationRequestBase
    {
        /// <summary>
        /// The customer email of the reservation.
        /// </summary>
        [Required]
        public string CustomerEmail { get; init; }
    }
}
#nullable enable warnings
