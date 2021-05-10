using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CancunSurvivor.Booking.Api.Abstractions.Models;

namespace CancunSurvivor.Booking.Api.Abstractions
{
    /// <summary>
    /// The service definition.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntity"/> being operated on by this service.</typeparam>
    public interface IService<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// List all <see cref="IEntity"/> in the data store.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ServiceResponse{T}"/>.</returns>
        Task<ServiceResponse<IEnumerable<TEntity>>> ListAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get an <see cref="IEntity"/> from the data store by id.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ServiceResponse{T}"/>.</returns>
        Task<ServiceResponse<TEntity>> GetAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new <see cref="IEntity"/> in the data store, if valid.
        /// </summary>
        /// <param name="request">The <see cref="IEntityRequest"/> to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ServiceResponse{T}"/>.</returns>
        Task<ServiceResponse<TEntity>> CreateAsync(IEntityRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update an existing <see cref="IEntity"/> in the data store, if valid.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/>.</param>
        /// <param name="request">The <see cref="IEntityRequest"/> to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ServiceResponse{T}"/>.</returns>
        Task<ServiceResponse<TEntity>> UpdateAsync(Guid id, IEntityRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an existing <see cref="IEntity"/> from the data store.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ServiceResponse{T}"/>.</returns>
        Task<ServiceResponse<TEntity>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
