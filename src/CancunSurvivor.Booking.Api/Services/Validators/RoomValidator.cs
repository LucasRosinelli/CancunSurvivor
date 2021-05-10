using CancunSurvivor.Booking.Api.Abstractions.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CancunSurvivor.Booking.Api.Services.Validators
{
    /// <summary>
    /// The <see cref="Room"/> validator.
    /// </summary>
    public class RoomValidator : AbstractValidator<Room>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomValidator"/> class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Validations:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// Number:
        /// <list type="bullet">
        /// <item>Must not be empty.</item>
        /// <item>Must not exceed 100 characters long.</item>
        /// </list>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public RoomValidator(ILogger<RoomValidator> logger)
        {
            const int NameMaxLength = 100;

            RuleFor(room => room.Name)
                .NotEmpty()
                .WithMessage($"The {nameof(Room.Name)} field is required")
                .OnFailure(failure =>
                    logger.LogWarning("The {FieldName} field is required", nameof(Room.Name)))
                .MaximumLength(NameMaxLength)
                .WithMessage($"The max length allowed for the {nameof(Room.Name)} field is {NameMaxLength}")
                .OnFailure(failure =>
                    logger.LogWarning("The max length allowed for the {FieldName} field is {FieldMaxLength}", nameof(Room.Name), NameMaxLength));
        }
    }
}
