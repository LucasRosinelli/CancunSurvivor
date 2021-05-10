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
    /// Rooms endpoints.
    /// </summary>
    [ApiController]
    [Route("rooms")]
    [Produces(MediaTypeNames.Application.Json)]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomController"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IRoomService"/>.</param>
        /// <param name="mapper">The <see cref="IMapper"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public RoomController(IRoomService service, IMapper mapper, ILogger<RoomController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all rooms.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RoomResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var serviceResponse = await _service.ListAllAsync(cancellationToken);

            if (serviceResponse.Result == ServiceResult.Ok)
            {
                return this.CreateApiResult(serviceResponse, successResult: () =>
                {
                    IEnumerable<RoomResponse> result = serviceResponse.Value!.Select(x => CreateRoomResponse(x));

                    return Ok(result);
                });
            }

            const string problemMessage = "A problem occurred in your request";
            _logger.LogInformation(problemMessage);
            return Problem(problemMessage);
        }

        /// <summary>
        /// Get a room by id.
        /// </summary>
        /// <param name="id">The room id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoomResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var serviceResponse = await _service.GetAsync(id, cancellationToken);

            return this.CreateApiResult(serviceResponse, successResult: () =>
            {
                RoomResponse result = CreateRoomResponse(serviceResponse.Value!);

                return Ok(result);
            });
        }

        /// <summary>
        /// Update an existing room.
        /// </summary>
        /// <param name="id">The room id.</param>
        /// <param name="room">The room to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] RoomUpdateRequest room, CancellationToken cancellationToken = default)
        {
            var serviceResponse = await _service.UpdateAsync(id, room, cancellationToken);

            return this.CreateApiResult(serviceResponse);
        }

        private RoomResponse CreateRoomResponse(Room room)
        {
            return _mapper.Map<RoomResponse>(room);
        }
    }
}
