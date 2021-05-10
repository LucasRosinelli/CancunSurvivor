using System.Collections.Generic;
using System.Linq;
using CancunSurvivor.Booking.Api.Enums;
using FluentValidation.Results;

namespace CancunSurvivor.Booking.Api.Abstractions
{
    public class ServiceResponse<T>
        where T : class
    {
        public T? Value { get; set; }

        public ServiceResult Result { get; set; }

        public bool HasError
        {
            get
            {
                return Errors.Any();
            }
        }

        public IEnumerable<ValidationFailure> Errors { get; set; } = new List<ValidationFailure>();
    }
}
