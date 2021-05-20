using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CancunSurvivor.Booking.Api.Abstractions;
using CancunSurvivor.Booking.Api.Abstractions.Models;
using CancunSurvivor.Booking.Api.Controllers;
using CancunSurvivor.Booking.Api.Enums;
using CancunSurvivor.Booking.Api.ViewModels.Profiles;
using CancunSurvivor.Booking.Api.ViewModels.Requests;
using CancunSurvivor.Booking.Api.ViewModels.Responses;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CancunSurvivor.Booking.Api.UnitTest.Controllers
{
    public class ReservationControllerTests
    {
        private readonly Mock<IReservationService> _mockService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ReservationController>> _mockLogger;
        private readonly ReservationController _controller;

        public ReservationControllerTests()
        {
            _mockService = new Mock<IReservationService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ReservationController>>();

            var mapper = new MapperConfiguration(config => config.AddProfile<ReservationProfile>());
            _controller = new ReservationController(_mockService.Object, mapper.CreateMapper(), _mockLogger.Object);
        }

        [Fact]
        public void Initialize_WhenServiceIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IReservationService service = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ReservationController(service, _mockMapper.Object, _mockLogger.Object));
        }

        [Fact]
        public void Initialize_WhenMapperIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IMapper mapper = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ReservationController(_mockService.Object, mapper, _mockLogger.Object));
        }

        [Fact]
        public void Initialize_WhenLoggerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<ReservationController> logger = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ReservationController(_mockService.Object, _mockMapper.Object, logger));
        }

        [Fact]
        public async Task Get_WhenServiceResultIsOk_ShouldReturnSuccessfully()
        {
            // Arrange
            var rnd = new Random();
            var serviceValue = Enumerable.Range(1, rnd.Next(1, 8))
                .Select(x => new Reservation
                {
                    Id = Guid.NewGuid(),
                    CustomerEmail = $"fake-{Guid.NewGuid():n}@domain.com",
                    CheckInDate = DateTime.Now.Date.AddDays(x),
                    CheckOutDate = DateTime.Now.Date.AddDays(x),
                    RoomId = Guid.NewGuid(),
                }).ToList();
            var serviceResponse = new ServiceResponse<IEnumerable<Reservation>>
            {
                Result = ServiceResult.Ok,
                Value = serviceValue,
            };
            _mockService
                .Setup(x => x.ListAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.NotNull(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okObjectResult.StatusCode);
            List<ReservationResponse> okObjectResultValue = Assert.IsAssignableFrom<IEnumerable<ReservationResponse>>(okObjectResult.Value).ToList();
            Assert.All(okObjectResultValue, x => Assert.NotNull(serviceValue.FirstOrDefault(s => s.Id == x.Id && s.CustomerEmail == x.CustomerEmail && s.CheckInDate.Date == x.CheckInDate.Date && s.CheckOutDate.Date == x.CheckOutDate.Date && s.RoomId == x.RoomId)));
        }

        [Theory]
        [InlineData(ServiceResult.NoContent)]
        [InlineData(ServiceResult.NotFound)]
        [InlineData(ServiceResult.ValidationFailed)]
        public async Task Get_WhenServiceResultIsNotOk_ShouldReturnProblemObjectResult(ServiceResult serviceResult)
        {
            // Arrange
            var serviceValue = new ServiceResponse<IEnumerable<Reservation>>
            {
                Result = serviceResult,
            };
            _mockService
                .Setup(x => x.ListAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceValue);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.NotNull(result);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var objectResultValue = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("A problem occurred in your request", objectResultValue.Detail);
        }

        [Fact]
        public async Task GetById_WhenServiceResultIsOk_ShouldReturnSuccessfully()
        {
            // Arrange
            var serviceValue = new Reservation
            {
                Id = Guid.NewGuid(),
                CustomerEmail = $"fake-{Guid.NewGuid():n}@domain.com",
                CheckInDate = DateTime.Now.Date.AddDays(1),
                CheckOutDate = DateTime.Now.Date.AddDays(3),
                RoomId = Guid.NewGuid(),
            };
            var serviceResponse = new ServiceResponse<Reservation>
            {
                Result = ServiceResult.Ok,
                Value = serviceValue,
            };
            _mockService
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetById(serviceValue.Id);

            // Assert
            Assert.NotNull(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okObjectResult.StatusCode);
            var okObjectResultValue = Assert.IsType<ReservationResponse>(okObjectResult.Value);
            Assert.Equal(serviceValue.Id, okObjectResultValue.Id);
            Assert.Equal(serviceValue.CustomerEmail, okObjectResultValue.CustomerEmail);
            Assert.Equal(serviceValue.CheckInDate.Date, okObjectResultValue.CheckInDate.Date);
            Assert.Equal(serviceValue.CheckOutDate.Date, okObjectResultValue.CheckOutDate.Date);
            Assert.Equal(serviceValue.RoomId, okObjectResultValue.RoomId);
        }

        [Fact]
        public async Task GetById_WhenServiceResultIsNotFound_ShouldReturnProblemObjectResult()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<Reservation>
            {
                Result = ServiceResult.NotFound,
            };
            _mockService
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetById(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Update_WhenServiceResultIsOk_ShouldReturnSuccessfully()
        {
            // Arrange
            var request = new ReservationUpdateRequest
            {
                CheckInDate = DateTime.Now.Date.AddDays(1),
                CheckOutDate = DateTime.Now.Date.AddDays(3),
            };
            var serviceValue = new Reservation
            {
                Id = Guid.NewGuid(),
                CustomerEmail = $"fake-{Guid.NewGuid():n}@domain.com",
                CheckInDate = DateTime.Now.Date.AddDays(1),
                CheckOutDate = DateTime.Now.Date.AddDays(3),
                RoomId = Guid.NewGuid(),
            };
            var serviceResponse = new ServiceResponse<Reservation>
            {
                Result = ServiceResult.Ok,
                Value = serviceValue,
            };
            _mockService
                .Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEntityRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Update(serviceValue.Id, request);

            // Assert
            Assert.NotNull(result);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }

        [Fact]
        public async Task Update_WhenServiceResultIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var request = new ReservationUpdateRequest
            {
                CheckInDate = DateTime.Now.Date.AddDays(1),
                CheckOutDate = DateTime.Now.Date.AddDays(3),
            };
            var serviceResponse = new ServiceResponse<Reservation>
            {
                Result = ServiceResult.NotFound,
            };
            _mockService
                .Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEntityRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), request);

            // Assert
            Assert.NotNull(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Update_WhenServiceResultIsValidationFailed_ShouldReturnNotFoundResult()
        {
            // Arrange
            var request = new ReservationUpdateRequest
            {
                CheckInDate = DateTime.Now.Date.AddDays(1),
                CheckOutDate = DateTime.Now.Date.AddDays(3),
            };
            var rnd = new Random();
            var serviceErrors = Enumerable.Range(1, rnd.Next(1, 4))
                .Select(x => new ValidationFailure($"some-property-{x}", $"an error message {Guid.NewGuid():n}")).ToList();
            var serviceResponse = new ServiceResponse<Reservation>
            {
                Result = ServiceResult.ValidationFailed,
                Errors = serviceErrors,
            };
            _mockService
                .Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEntityRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), request);

            // Assert
            Assert.NotNull(result);
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestObjectResult.StatusCode);
            List<ValidationFailure> badRequestObjectResultValue = Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequestObjectResult.Value).ToList();
            Assert.All(badRequestObjectResultValue, x => Assert.NotNull(serviceErrors.FirstOrDefault(s => s.PropertyName == x.PropertyName && s.ErrorMessage == x.ErrorMessage)));
        }
    }
}
