using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CancunSurvivor.Booking.Api.Abstractions;
using CancunSurvivor.Booking.Api.Abstractions.Models;
using CancunSurvivor.Booking.Api.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CancunSurvivor.Booking.Api.Services
{
    /// <summary>
    /// Represents the service to manage <see cref="Reservation"/>.
    /// </summary>
    public class ReservationService : BaseService<Reservation>, IReservationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationService"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="AppDbContext"/>.</param>
        /// <param name="validator">The <see cref="AbstractValidator{T}"/> of the <see cref="Reservation"/>.</param>
        /// <param name="mapper">The <see cref="IMapper"/> for mapping from and to <see cref="Reservation"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public ReservationService(AppDbContext dbContext, AbstractValidator<Reservation> validator, IMapper mapper, ILogger<ReservationService> logger)
            : base(dbContext, validator, mapper, logger)
        {
        }

        /// <summary>
        /// Validate the <see cref="Reservation"/> creation using custom validation.
        /// </summary>
        /// <param name="entity">The <see cref="Reservation"/> to validate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ValidationResult"/>.</returns>
        protected override async Task<ValidationResult> InternalCreateValidateAsync(Reservation entity, CancellationToken cancellationToken = default)
        {
            var result = await base.InternalCreateValidateAsync(entity, cancellationToken);

            result = await InternalValidateAsync(result, entity, cancellationToken);

            return result;
        }

        /// <summary>
        /// Validate the <see cref="Reservation"/> update using custom validation.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/>.</param>
        /// <param name="entity">The <see cref="Reservation"/> to validate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ValidationResult"/>.</returns>
        protected override async Task<ValidationResult> InternalUpdateValidateAsync(Guid id, Reservation entity, CancellationToken cancellationToken = default)
        {
            var result = await base.InternalUpdateValidateAsync(id, entity, cancellationToken);

            result = await InternalValidateAsync(result, entity, cancellationToken);

            return result;
        }

        private async Task<ValidationResult> InternalValidateAsync(ValidationResult result, Reservation reservation, CancellationToken cancellationToken = default)
        {
            if (result.IsValid)
            {
                var existing = await ExistingReservationsForValidationsAsync(reservation, cancellationToken);
                bool isOverlapping = IsOverlapping(existing, reservation);
                if (isOverlapping)
                {
                    result.Errors.Add(new ValidationFailure(nameof(Reservation.CheckInDate), "The chosen check-in/check-out dates are not available: overlapping other reservation(s)"));
                }

                bool hasImmediatelyCloserReservation = HasImmediatelyCloserReservation(existing, reservation);
                if (hasImmediatelyCloserReservation)
                {
                    result.Errors.Add(new ValidationFailure(nameof(Reservation.CheckInDate), "You can't make subsequent reservations without a full-day between them"));
                }
            }

            return result;
        }

        private async Task<IList<Reservation>> ExistingReservationsForValidationsAsync(Reservation reservation, CancellationToken cancellationToken = default)
        {
            DateTime begin = reservation.CheckInDate.AddDays(-1).Date;
            DateTime end = reservation.CheckOutDate.AddDays(1).Date;

            IList<Reservation> reservationsForValidation = await EntitySet
                .AsNoTracking()
                .Where(x => x.Id != reservation.Id && x.CheckInDate.Date <= end && begin <= x.CheckOutDate.Date)
                .ToListAsync(cancellationToken);

            return reservationsForValidation;
        }

        private bool IsOverlapping(IList<Reservation> reservations, Reservation entity)
        {
            bool isOverlapping = reservations
                .Any(x => x.CheckInDate.Date <= entity.CheckOutDate.Date && entity.CheckInDate.Date <= x.CheckOutDate.Date);

            return isOverlapping;
        }

        private bool HasImmediatelyCloserReservation(IList<Reservation> reservations, Reservation entity)
        {
            bool hasImmediatelyCloserReservation = reservations
                .Any(x => x.CustomerEmail.Equals(entity.CustomerEmail, StringComparison.OrdinalIgnoreCase) && (x.CheckInDate.Date > entity.CheckOutDate.Date || entity.CheckInDate.Date > x.CheckOutDate.Date));

            return hasImmediatelyCloserReservation;
        }
    }
}
