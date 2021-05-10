using System;
using CancunSurvivor.Booking.Api.Abstractions;
using CancunSurvivor.Booking.Api.Abstractions.Models;
using CancunSurvivor.Booking.Api.Enums;
using CancunSurvivor.Booking.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CancunSurvivor.Booking.Api.UnitTest.Extensions
{
    public class ControllerExtensionsTests
    {
        private readonly Mock<ControllerBase> _mockController;

        public ControllerExtensionsTests()
        {
            _mockController = new Mock<ControllerBase>();
        }

        [Fact]
        public void CreateApiResult_WhenControllerIsNull_ThrowsNullReferenceException()
        {
            // Arrange
            ControllerBase controller = null!;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => ControllerExtensions.CreateApiResult(controller, new ServiceResponse<Room>()));
        }

        [Fact]
        public void CreateApiResult_WhenServiceResponseIsNull_ThrowsNullReferenceException()
        {
            // Arrange
            ServiceResponse<Room> serviceResponse = null!;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _mockController.Object.CreateApiResult(serviceResponse));
        }

        [Theory]
        [InlineData(ServiceResult.Ok)]
        [InlineData(ServiceResult.NoContent)]
        public void CreateApiResult_WhenServiceResultIsSuccessWithNoSuccessDelegateSpecified_ShouldCallNoContent(ServiceResult serviceResult)
        {
            // Arrange
            var serviceResponse = new ServiceResponse<Room>
            {
                Result = serviceResult,
            };

            // Act
            _mockController.Object.CreateApiResult(serviceResponse);

            // Assert
            _mockController.Verify(x => x.NoContent(), Times.Once());
        }

        [Theory]
        [InlineData(ServiceResult.Ok)]
        [InlineData(ServiceResult.NoContent)]
        public void CreateApiResult_WhenServiceResultIsSuccess_ShouldInvokeSuccessResult(ServiceResult serviceResult)
        {
            // Arrange
            var actionResultValue = new Room
            {
                Id = Guid.NewGuid(),
                Name = $"any-name-{serviceResult}",
            };
            var serviceResponse = new ServiceResponse<Room>
            {
                Result = serviceResult,
            };

            // Act
            IActionResult result = _mockController.Object.CreateApiResult(serviceResponse, successResult: () => new OkObjectResult(actionResultValue));

            // Assert
            Assert.NotNull(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okObjectResult);
            var okObjectResultValue = Assert.IsType<Room>(okObjectResult.Value);
            Assert.NotNull(okObjectResultValue);
            Assert.Equal(actionResultValue.Id, okObjectResultValue.Id);
            Assert.Equal(actionResultValue.Name, okObjectResultValue.Name);
        }

        [Fact]
        public void CreateApiResult_WhenServiceResultIsNotFound_ShouldCallNotFound()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<Room>
            {
                Result = ServiceResult.NotFound,
            };

            // Act
            _mockController.Object.CreateApiResult(serviceResponse);

            // Assert
            _mockController.Verify(x => x.NotFound(), Times.Once());
        }

        [Fact]
        public void CreateApiResult_WhenServiceResultIsValidationFailed_ShouldCallBadRequest()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<Room>
            {
                Result = ServiceResult.ValidationFailed,
            };

            // Act
            _mockController.Object.CreateApiResult(serviceResponse);

            // Assert
            _mockController.Verify(x => x.BadRequest(It.IsAny<object>()), Times.Once());
        }

        [Theory]
        [InlineData((ServiceResult)4)]
        [InlineData((ServiceResult)50)]
        public void CreateApiResult_WhenServiceResultIsNotRecognized_ThrowsInvalidOperationException(ServiceResult serviceResult)
        {
            // Arrange
            var serviceResponse = new ServiceResponse<Room>
            {
                Result = serviceResult,
            };

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => _mockController.Object.CreateApiResult(serviceResponse));

            // Assert
            Assert.Equal($"The service response result {serviceResponse.Result} is not supported", exception.Message);
        }
    }
}
