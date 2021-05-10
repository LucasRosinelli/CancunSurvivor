using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CancunSurvivor.Booking.Api.Abstractions.Models;
using CancunSurvivor.Booking.Api.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CancunSurvivor.Booking.Api.Abstractions
{
    /// <summary>
    /// Represents a base service.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntity"/> being operated on by this service.</typeparam>
    public abstract class BaseService<TEntity> : IService<TEntity>
        where TEntity : class, IEntity
    {
        private readonly string _entityName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{TEntity}"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="AppDbContext"/>.</param>
        /// <param name="validator">The <see cref="AbstractValidator{T}"/> of the <see cref="IEntity"/>.</param>
        /// <param name="mapper">The <see cref="IMapper"/> for mapping from and to <see cref="IEntity"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public BaseService(AppDbContext dbContext, AbstractValidator<TEntity> validator, IMapper mapper, ILogger<BaseService<TEntity>> logger)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _entityName = typeof(TEntity).Name;
        }

        /// <summary>
        /// The <see cref="AppDbContext"/>.
        /// </summary>
        protected AppDbContext DbContext { get; private set; }

        /// <summary>
        /// The set on <see cref="IEntity"/> in the data store.
        /// </summary>
        protected DbSet<TEntity> EntitySet
        {
            get
            {
                return DbContext.Set<TEntity>();
            }
        }

        /// <summary>
        /// The <see cref="AbstractValidator{T}"/> of the <see cref="IEntity"/>.
        /// </summary>
        protected AbstractValidator<TEntity> Validator { get; private set; }

        /// <summary>
        /// The <see cref="IMapper"/> for mapping from and to <see cref="IEntity"/>.
        /// </summary>
        protected IMapper Mapper { get; private set; }

        /// <summary>
        /// The <see cref="ILogger"/>.
        /// </summary>
        protected ILogger<BaseService<TEntity>> Logger { get; private set; }

        /// <inheritdoc/>
        public virtual async Task<ServiceResponse<IEnumerable<TEntity>>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<TEntity> result = await EntitySet.AsNoTracking().ToListAsync(cancellationToken);

            return ServiceResponseFactory.Ok(result);
        }

        /// <inheritdoc/>
        public virtual async Task<ServiceResponse<TEntity>> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            TEntity? result = await EntitySet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            return CreateServiceResponse(result);
        }

        /// <inheritdoc/>
        public virtual async Task<ServiceResponse<TEntity>> CreateAsync(IEntityRequest request, CancellationToken cancellationToken = default)
        {
            TEntity entity = Mapper.Map<TEntity>(request);

            ValidationResult validationResult = await InternalCreateValidateAsync(entity, cancellationToken);

            if (!validationResult.IsValid)
            {
                return ServiceResponseFactory.ValidationFailed<TEntity>(validationResult, _entityName, Logger);
            }

            await EntitySet.AddAsync(entity, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);

            return ServiceResponseFactory.Ok(entity);
        }

        /// <inheritdoc/>
        public virtual async Task<ServiceResponse<TEntity>> UpdateAsync(Guid id, IEntityRequest request, CancellationToken cancellationToken = default)
        {
            TEntity entity = Mapper.Map<TEntity>(request);

            ValidationResult validationResult = await InternalUpdateValidateAsync(id, entity, cancellationToken);

            if (!validationResult.IsValid)
            {
                return ServiceResponseFactory.ValidationFailed<TEntity>(validationResult, _entityName, Logger);
            }

            var storedEntity = await GetAsync(id, cancellationToken);

            DbContext.Update(storedEntity);
            await DbContext.SaveChangesAsync(cancellationToken);

            return ServiceResponseFactory.Ok(entity);
        }

        /// <inheritdoc/>
        public virtual async Task<ServiceResponse<TEntity>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var storedEntity = await EntitySet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (storedEntity is null)
            {
                return ServiceResponseFactory.NotFound<TEntity>();
            }

            DbContext.Remove(storedEntity);
            await DbContext.SaveChangesAsync(cancellationToken);

            return ServiceResponseFactory.NoContent<TEntity>();
        }

        /// <summary>
        /// Validate the <see cref="IEntity"/> creation using the <see cref="BaseService{TEntity}.Validator"/>.
        /// </summary>
        /// <param name="entity">The <see cref="IEntity"/> to validate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ValidationResult"/>.</returns>
        protected virtual Task<ValidationResult> InternalCreateValidateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return InternalValidateAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Validate the <see cref="IEntity"/> update using the <see cref="BaseService{TEntity}.Validator"/>.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/>.</param>
        /// <param name="entity">The <see cref="IEntity"/> to validate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ValidationResult"/>.</returns>
        protected virtual Task<ValidationResult> InternalUpdateValidateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default)
        {
            return InternalValidateAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Validate the <see cref="IEntity"/> using the <see cref="BaseService{TEntity}.Validator"/>.
        /// </summary>
        /// <param name="entity">The <see cref="IEntity"/> to validate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ValidationResult"/>.</returns>
        private async Task<ValidationResult> InternalValidateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            ValidationResult validationResult = await Validator.ValidateAsync(entity, cancellationToken);

            return validationResult;
        }

        /// <summary>
        /// Creates the service response.
        /// </summary>
        /// <typeparam name="T">The type of the service response value.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>A <see cref="ServiceResponseFactory.Ok{T}(T)"/> if <paramref name="entity"/> is not <see langword="null"/>; otherwise, <see cref="ServiceResponseFactory.NotFound{T}"/>.</returns>
        private ServiceResponse<T> CreateServiceResponse<T>(T? entity)
            where T : class
        {
            if (entity == null)
            {
                return ServiceResponseFactory.NotFound<T>();
            }

            return ServiceResponseFactory.Ok(entity);
        }
    }
}
