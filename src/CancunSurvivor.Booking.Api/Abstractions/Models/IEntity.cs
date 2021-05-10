using System;

namespace CancunSurvivor.Booking.Api.Abstractions.Models
{
    /// <summary>
    /// The entity definition.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The id of the entity.
        /// </summary>
        Guid Id { get; set; }
    }
}
