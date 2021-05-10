using System;
using CancunSurvivor.Booking.Api.Abstractions.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CancunSurvivor.Booking.Api.Services.Validators
{
    /// <summary>
    /// The <see cref="Reservation"/> validator.
    /// </summary>
    public class ReservationValidator : AbstractValidator<Reservation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationValidator"/> class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// All date-based rules discard timezones and uses the standard from the system where the application is running on.
        /// In real-world scenarios I would avoid doing this as the application my run worldwide with different timezones for
        /// both rooms and end-users that could lead to inconsistency or unexpected behaviors.
        /// </para>
        /// <para>
        /// Validations:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// Customer email:
        /// <list type="bullet">
        /// <item>Must not be empty.</item>
        /// <item>Must not exceed 500 characters long.</item>
        /// <item>Must be a valid email address.</item>
        /// </list>
        /// </item>
        /// <item>
        /// Check-in date:
        /// <list type="bullet">
        /// <item>Must be greater than the current date.</item>
        /// </list>
        /// </item>
        /// <item>
        /// Check-out date:
        /// <list type="bullet">
        /// <item>Must be greater than or equal to the check-in date.</item>
        /// <item>Must not exceed current date + 30 days (the 30th day is allowed).</item>
        /// <item>Comparing to check-in date, the difference must not exceed 3 days total. Both check-in date and check-out date are fully included (from 00:00 to 23:59:59).</item>
        /// </list>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public ReservationValidator(ILogger<ReservationValidator> logger)
        {
            const int CustomerEmailMaxLength = 500;
            const int ReservationDaysInAdvance = 30;
            const int MaximumAllowedDaysStay = 3;

            RuleFor(reservation => reservation.CustomerEmail)
                .NotEmpty()
                .WithMessage($"The {nameof(Reservation.CustomerEmail)} field is required")
                .OnFailure(customClaim =>
                    logger.LogWarning("The {FieldName} field is required", nameof(Reservation.CustomerEmail)))
                .MaximumLength(CustomerEmailMaxLength)
                .WithMessage($"The max length allowed for the {nameof(Reservation.CustomerEmail)} is {CustomerEmailMaxLength}")
                .OnFailure(customClaim =>
                    logger.LogWarning("The max length allowed for the {FieldName} is {FieldMaxLength}", nameof(Reservation.CustomerEmail), CustomerEmailMaxLength))
                .EmailAddress()
                .WithMessage($"The {nameof(Reservation.CustomerEmail)} must be a valid email")
                .OnFailure(customClaim =>
                    logger.LogWarning("The {FieldName} must be a valid email", nameof(Reservation.CustomerEmail)));

            var todayStarts = DateTime.Now.Date;
            var todayEnds = todayStarts.AddDays(1).AddMilliseconds(-1);
            var maximumLimitInAdvance = todayStarts.AddDays(ReservationDaysInAdvance);

            RuleFor(reservation => reservation.CheckInDate.Date)
                .GreaterThan(todayEnds)
                .WithMessage($"The {nameof(Reservation.CheckInDate)} field must be greater than current date (today)")
                .OnFailure(failure =>
                    logger.LogWarning("The {FieldName} field must be greater than current date (today)", nameof(Reservation.CheckInDate)));

            RuleFor(reservation => reservation.CheckOutDate.Date)
                .GreaterThanOrEqualTo(reservation => reservation.CheckInDate.Date)
                .WithMessage($"The {nameof(Reservation.CheckOutDate)} field must be greater than or equal to {nameof(Reservation.CheckInDate)} field")
                .OnFailure(failure =>
                    logger.LogWarning("The {FieldName} field must be greater than or equal to {FieldName} field", nameof(Reservation.CheckOutDate), nameof(Reservation.CheckInDate)))
                .LessThanOrEqualTo(reservation => maximumLimitInAdvance)
                .WithMessage($"The stay can't be reserved more than {ReservationDaysInAdvance} days in advance")
                .OnFailure(failure =>
                    logger.LogWarning("The stay can't be reserved more than {ReservationDaysInAdvance} days in advance", ReservationDaysInAdvance));

            RuleFor(reservation => reservation.CheckOutDate.Date.Subtract(reservation.CheckInDate.Date).Days)
                .LessThan(MaximumAllowedDaysStay)
                .WithMessage($"The stay can't be longer than {MaximumAllowedDaysStay} days")
                .OnFailure(failure =>
                    logger.LogWarning("The stay can't be longer than {MaximumAllowedDaysStay} days", MaximumAllowedDaysStay));
        }
    }
}
