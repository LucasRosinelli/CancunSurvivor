using System;
using CancunSurvivor.Booking.Api.Abstractions;
using CancunSurvivor.Booking.Api.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CancunSurvivor.Booking.Api.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ControllerBase"/>.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Create an API result based on <see cref="ServiceResponse{T}"/>.
        /// </summary>
        /// <param name="controller">The <see cref="ControllerBase"/>.</param>
        /// <param name="serviceResponse">The <see cref="ServiceResponse{T}"/>.</param>
        /// <param name="successResult">The delegate to call when success: <see cref="ServiceResult.Ok"/> or <see cref="ServiceResult.NoContent"/>.</param>
        /// <typeparam name="T">The type of the service response value.</typeparam>
        /// <returns>An <see cref="IActionResult"/> based on the <see cref="ServiceResponse{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The given <see cref="ServiceResult"/> at <see cref="ServiceResponse{T}"/> is not supported.</exception>
        public static IActionResult CreateApiResult<T>(this ControllerBase controller, ServiceResponse<T> serviceResponse, Func<IActionResult> successResult)
            where T : class
        {
            return serviceResponse.Result switch
            {
                ServiceResult.Ok or ServiceResult.NoContent => successResult.Invoke(),
                ServiceResult.NotFound => controller.NotFound(),
                ServiceResult.ValidationFailed => controller.BadRequest(serviceResponse.Errors),
                _ => throw new InvalidOperationException($"The service response result {serviceResponse.Result} is not supported"),
            };
        }

        /// <summary>
        /// Create an API result based on <see cref="ServiceResponse{T}"/>.
        /// </summary>
        /// <remarks>The success result is the <see cref="ControllerBase.NoContent"/>.</remarks>
        /// <param name="controller">The <see cref="ControllerBase"/>.</param>
        /// <param name="serviceResponse">The <see cref="ServiceResponse{T}"/>.</param>
        /// <typeparam name="T">The type of the service response value.</typeparam>
        /// <returns>An <see cref="IActionResult"/> based on the <see cref="ServiceResponse{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The given <see cref="ServiceResult"/> at <see cref="ServiceResponse{T}"/> is not supported.</exception>
        public static IActionResult CreateApiResult<T>(this ControllerBase controller, ServiceResponse<T> serviceResponse)
            where T : class
        {
            return controller.CreateApiResult(serviceResponse, successResult: () => controller.NoContent());
        }
    }
}
