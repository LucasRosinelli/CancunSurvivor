using System;

namespace CancunSurvivor.Booking.Api.Abstractions.Models
{
    /// <summary>
    /// Represents a base entity.
    /// </summary>
    public abstract class BaseEntity : IEntity
    {
        /// <summary>
        /// The id of the entity.
        /// </summary>
        public Guid Id { get; set; }
    }
}
