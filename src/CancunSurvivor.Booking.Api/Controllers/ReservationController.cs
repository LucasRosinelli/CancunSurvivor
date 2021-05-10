using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CancunSurvivor.Booking.Api.Abstractions;
using CancunSurvivor.Booking.Api.Abstractions.Models;
using CancunSurvivor.Booking.Api.Enums;
using CancunSurvivor.Booking.Api.Extensions;
using CancunSurvivor.Booking.Api.ViewModels.Requests;
using CancunSurvivor.Booking.Api.ViewModels.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CancunSurvivor.Booking.Api.Controllers
{
    /// <summary>
    /// Reservations endpoints.
    /// </summary>
    [ApiController]
    [Route("reservations")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservationController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationController"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IReservationService"/>.</param>
        /// <param name="mapper">The <see cref="IMapper"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public ReservationController(IReservationService service, IMapper mapper, ILogger<ReservationController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all reservations.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReservationResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var serviceResponse = await _service.ListAllAsync(cancellationToken);

            if (serviceResponse.Result == ServiceResult.Ok)
            {
                return this.CreateApiResult(serviceResponse, successResult: () =>
                {
                    IEnumerable<ReservationResponse> result = serviceResponse.Value!.Select(x => CreateReservationResponse(x));

                    return Ok(result);
                });
            }

            const string problemMessage = "A problem occurred in your request";
            _logger.LogInformation(problemMessage);
            return Problem(problemMessage);
        }

        /// <summary>
        /// Get a reservation by id.
        /// </summary>
        /// <param name="id">The reservation id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReservationResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var serviceResponse = await _service.GetAsync(id, cancellationToken);

            return this.CreateApiResult(serviceResponse, successResult: () =>
            {
                ReservationResponse result = CreateReservationResponse(serviceResponse.Value!);

                return Ok(result);
            });
        }

        /// <summary>
        /// Create a new reservation.
        /// </summary>
        /// <param name="reservation">The reservation to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReservationResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ReservationCreateRequest reservation, CancellationToken cancellationToken = default)
        {
            var serviceResponse = await _service.CreateAsync(reservation, cancellationToken);

            return this.CreateApiResult(serviceResponse, successResult: () =>
            {
                var result = CreateReservationResponse(serviceResponse.Value!);

                return Created(Url.ActionLink(action: nameof(GetById), values: new { Id = result.Id, }), result);
            });
        }

        /// <summary>
        /// Update an existing reservation.
        /// </summary>
        /// <param name="id">The reservation id.</param>
        /// <param name="reservation">The reservation to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReservationUpdateRequest reservation, CancellationToken cancellationToken = default)
        {
            var serviceResponse = await _service.UpdateAsync(id, reservation, cancellationToken);

            return this.CreateApiResult(serviceResponse);
        }

        /// <summary>
        /// Delete an existing reservation.
        /// </summary>
        /// <param name="id">The reservation id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken = default)
        {
            var serviceResponse = await _service.DeleteAsync(id, cancellationToken);

            return this.CreateApiResult(serviceResponse);
        }

        private ReservationResponse CreateReservationResponse(Reservation reservation)
        {
            return _mapper.Map<ReservationResponse>(reservation);
        }
    }
}
