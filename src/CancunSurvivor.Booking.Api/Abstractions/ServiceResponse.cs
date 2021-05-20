using System.Collections.Generic;
using System.Linq;
using CancunSurvivor.Booking.Api.Enums;
using FluentValidation.Results;

namespace CancunSurvivor.Booking.Api.Abstractions
{
    /// <summary>
    /// Represents the response from the service layer.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Value"/>.</typeparam>
    public class ServiceResponse<T>
        where T : class
    {
        /// <summary>
        /// The response value.
        /// </summary>
        public T? Value { get; set; }

        /// <summary>
        /// The <see cref="ServiceResult"/>.
        /// </summary>
        public ServiceResult Result { get; set; }

        /// <summary>
        /// Indicates whether the response has errors or not based on <see cref="Errors"/>.
        /// </summary>
        public bool HasError
        {
            get
            {
                return Errors.Any();
            }
        }

        /// <summary>
        /// The validation errors.
        /// </summary>
        public IEnumerable<ValidationFailure> Errors { get; set; } = new List<ValidationFailure>();
    }
}
