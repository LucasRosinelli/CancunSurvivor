using AutoMapper;
using CancunSurvivor.Booking.Api.Abstractions;
using CancunSurvivor.Booking.Api.Abstractions.Models;
using CancunSurvivor.Booking.Api.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CancunSurvivor.Booking.Api.Services
{
    /// <summary>
    /// Represents the service to manage <see cref="Room"/>.
    /// </summary>
    public class RoomService : BaseService<Room>, IRoomService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomService"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="AppDbContext"/>.</param>
        /// <param name="validator">The <see cref="AbstractValidator{T}"/> of the <see cref="Room"/>.</param>
        /// <param name="mapper">The <see cref="IMapper"/> for mapping from and to <see cref="Room"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public RoomService(AppDbContext dbContext, AbstractValidator<Room> validator, IMapper mapper, ILogger<RoomService> logger)
            : base(dbContext, validator, mapper, logger)
        {
        }
    }
}
