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
    public class RoomControllerTests
    {
        private readonly Mock<IRoomService> _mockService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RoomController>> _mockLogger;

        public RoomControllerTests()
        {
            _mockService = new Mock<IRoomService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<RoomController>>();
        }

        [Fact]
        public void Initialize_WhenServiceIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IRoomService service = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RoomController(service, _mockMapper.Object, _mockLogger.Object));
        }

        [Fact]
        public void Initialize_WhenMapperIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IMapper mapper = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RoomController(_mockService.Object, mapper, _mockLogger.Object));
        }

        [Fact]
        public void Initialize_WhenLoggerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<RoomController> logger = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RoomController(_mockService.Object, _mockMapper.Object, logger));
        }

        [Fact]
        public async Task Get_WhenServiceResultIsOk_ShouldReturnSuccessfully()
        {
            // Arrange
            var rnd = new Random();
            var serviceValue = Enumerable.Range(1, rnd.Next(1, 8))
                .Select(x => new Room
                {
                    Id = Guid.NewGuid(),
                    Name = $"any-room-{x}",
                }).ToList();
            var serviceResponse = new ServiceResponse<IEnumerable<Room>>
            {
                Result = ServiceResult.Ok,
                Value = serviceValue,
            };
            _mockService
                .Setup(x => x.ListAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);
            RoomController controller = CreateController();

            // Act
            var result = await controller.Get();

            // Assert
            Assert.NotNull(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okObjectResult.StatusCode);
            List<RoomResponse> okObjectResultValue = Assert.IsAssignableFrom<IEnumerable<RoomResponse>>(okObjectResult.Value).ToList();
            Assert.All(okObjectResultValue, x => Assert.NotNull(serviceValue.FirstOrDefault(s => s.Id == x.Id && s.Name == x.Name)));
        }

        [Theory]
        [InlineData(ServiceResult.NoContent)]
        [InlineData(ServiceResult.NotFound)]
        [InlineData(ServiceResult.ValidationFailed)]
        public async Task Get_WhenServiceResultIsNotOk_ShouldReturnProblemObjectResult(ServiceResult serviceResult)
        {
            // Arrange
            var serviceValue = new ServiceResponse<IEnumerable<Room>>
            {
                Result = serviceResult,
            };
            _mockService
                .Setup(x => x.ListAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceValue);
            RoomController controller = CreateController();

            // Act
            var result = await controller.Get();

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
            var serviceValue = new Room
            {
                Id = Guid.NewGuid(),
                Name = $"any-room-{Guid.NewGuid():n}",
            };
            var serviceResponse = new ServiceResponse<Room>
            {
                Result = ServiceResult.Ok,
                Value = serviceValue,
            };
            _mockService
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);
            RoomController controller = CreateController();

            // Act
            var result = await controller.GetById(serviceValue.Id);

            // Assert
            Assert.NotNull(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okObjectResult.StatusCode);
            var okObjectResultValue = Assert.IsType<RoomResponse>(okObjectResult.Value);
            Assert.Equal(serviceValue.Id, okObjectResultValue.Id);
            Assert.Equal(serviceValue.Name, okObjectResultValue.Name);
        }

        [Fact]
        public async Task GetById_WhenServiceResultIsNotFound_ShouldReturnProblemObjectResult()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<Room>
            {
                Result = ServiceResult.NotFound,
            };
            _mockService
                .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);
            RoomController controller = CreateController();

            // Act
            var result = await controller.GetById(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Update_WhenServiceResultIsOk_ShouldReturnSuccessfully()
        {
            // Arrange
            var request = new RoomUpdateRequest
            {
                Name = $"request-to-update-room-{Guid.NewGuid():n}",
            };
            var serviceValue = new Room
            {
                Id = Guid.NewGuid(),
                Name = $"any-updated-room-{Guid.NewGuid():n}",
            };
            var serviceResponse = new ServiceResponse<Room>
            {
                Result = ServiceResult.Ok,
                Value = serviceValue,
            };
            _mockService
                .Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEntityRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);
            RoomController controller = CreateController();

            // Act
            var result = await controller.Update(serviceValue.Id, request);

            // Assert
            Assert.NotNull(result);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }

        [Fact]
        public async Task Update_WhenServiceResultIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var request = new RoomUpdateRequest
            {
                Name = $"request-to-update-room-{Guid.NewGuid():n}",
            };
            var serviceResponse = new ServiceResponse<Room>
            {
                Result = ServiceResult.NotFound,
            };
            _mockService
                .Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEntityRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);
            RoomController controller = CreateController();

            // Act
            var result = await controller.Update(Guid.NewGuid(), request);

            // Assert
            Assert.NotNull(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Update_WhenServiceResultIsValidationFailed_ShouldReturnNotFoundResult()
        {
            // Arrange
            var request = new RoomUpdateRequest
            {
                Name = $"request-to-update-room-{Guid.NewGuid():n}",
            };
            var rnd = new Random();
            var serviceErrors = Enumerable.Range(1, rnd.Next(1, 4))
                .Select(x => new ValidationFailure($"some-property-{x}", $"an error message {Guid.NewGuid():n}")).ToList();
            var serviceResponse = new ServiceResponse<Room>
            {
                Result = ServiceResult.ValidationFailed,
                Errors = serviceErrors,
            };
            _mockService
                .Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEntityRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);
            RoomController controller = CreateController();

            // Act
            var result = await controller.Update(Guid.NewGuid(), request);

            // Assert
            Assert.NotNull(result);
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestObjectResult.StatusCode);
            List<ValidationFailure> badRequestObjectResultValue = Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequestObjectResult.Value).ToList();
            Assert.All(badRequestObjectResultValue, x => Assert.NotNull(serviceErrors.FirstOrDefault(s => s.PropertyName == x.PropertyName && s.ErrorMessage == x.ErrorMessage)));
        }

        private RoomController CreateController()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile<RoomProfile>());

            return new RoomController(_mockService.Object, mapper.CreateMapper(), _mockLogger.Object);
        }
    }
}
