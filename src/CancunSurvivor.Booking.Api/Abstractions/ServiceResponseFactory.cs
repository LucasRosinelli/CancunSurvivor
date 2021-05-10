using System.Linq;
using System.Text.Json;
using CancunSurvivor.Booking.Api.Enums;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CancunSurvivor.Booking.Api.Abstractions
{
    /// <summary>
    /// A factory for <see cref="ServiceResponse{T}"/>.
    /// </summary>
    public static class ServiceResponseFactory
    {
        /// <summary>
        /// Create an <see cref="ServiceResult.Ok"/> service response.
        /// </summary>
        /// <typeparam name="T">The type of the service response value.</typeparam>
        /// <param name="value">The service response value.</param>
        /// <returns>The <see cref="ServiceResponse{T}"/> with a <see cref="ServiceResult.Ok"/> and the informed value.</returns>
        internal static ServiceResponse<T> Ok<T>(T value)
            where T : class
        {
            return new ServiceResponse<T>
            {
                Value = value,
                Result = ServiceResult.Ok,
            };
        }

        /// <summary>
        /// Create an <see cref="ServiceResult.NotFound"/> service response.
        /// </summary>
        /// <typeparam name="T">The type of the service response value.</typeparam>
        /// <returns>The <see cref="ServiceResponse{T}"/> with a <see cref="ServiceResult.NotFound"/> and a default value.</returns>
        internal static ServiceResponse<T> NotFound<T>()
            where T : class
        {
            return new ServiceResponse<T>
            {
                Value = default,
                Result = ServiceResult.NotFound,
            };
        }

        /// <summary>
        /// Create an <see cref="ServiceResult.NoContent"/> service response.
        /// </summary>
        /// <typeparam name="T">The type of the service response value.</typeparam>
        /// <returns>The <see cref="ServiceResponse{T}"/> with a <see cref="ServiceResult.NoContent"/> and a default value.</returns>
        internal static ServiceResponse<T> NoContent<T>()
            where T : class
        {
            return new ServiceResponse<T>
            {
                Value = default,
                Result = ServiceResult.NoContent,
            };
        }

        /// <summary>
        /// Create an <see cref="ServiceResult.ValidationFailed"/> service response.
        /// </summary>
        /// <typeparam name="T">The type of the service response value.</typeparam>
        /// <param name="validationResult">The <see cref="ValidationResult"/>.</param>
        /// <param name="entityName">The entity name.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <returns>The <see cref="ServiceResponse{T}"/> with a <see cref="ServiceResult.ValidationFailed"/>, a default value and the occurred validation errors.</returns>
        internal static ServiceResponse<T> ValidationFailed<T>(ValidationResult validationResult, string entityName, ILogger? logger = null)
            where T : class
        {
            if (logger?.IsEnabled(LogLevel.Debug) == true)
            {
                var resultsForLog = validationResult.Errors.Select(e => new
                {
                    e.ErrorCode,
                    e.ErrorMessage,
                    e.PropertyName,
                    e.AttemptedValue,
                });
                logger.LogDebug("Validation failed for entity {EntityName}: {ValidationFailure}", entityName, JsonSerializer.Serialize(resultsForLog));
            }

            return new ServiceResponse<T>
            {
                Value = default,
                Result = ServiceResult.ValidationFailed,
                Errors = validationResult.Errors,
            };
        }
    }
}
