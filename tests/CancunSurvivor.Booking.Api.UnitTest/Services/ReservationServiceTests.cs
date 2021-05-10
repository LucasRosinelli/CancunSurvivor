using System;
using AutoMapper;
using CancunSurvivor.Booking.Api.Abstractions.Models;
using CancunSurvivor.Booking.Api.Repositories;
using CancunSurvivor.Booking.Api.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CancunSurvivor.Booking.Api.UnitTest.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<AbstractValidator<Reservation>> _mockValidator;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ReservationService>> _mockLogger;
        private readonly Mock<DbSet<Reservation>> _mockDbSetReservation;

        public ReservationServiceTests()
        {
            _mockValidator = new Mock<AbstractValidator<Reservation>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ReservationService>>();
            _mockDbSetReservation = new Mock<DbSet<Reservation>>();
        }

        [Fact]
        public void Initialize_WhenDbContextIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            AppDbContext dbContext = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ReservationService(dbContext, _mockValidator.Object, _mockMapper.Object, _mockLogger.Object));
        }

        [Fact]
        public void Initialize_WhenValidatorIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            AppDbContext dbContext = CreateDbContext();
            AbstractValidator<Reservation> validator = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ReservationService(dbContext, validator, _mockMapper.Object, _mockLogger.Object));
        }

        [Fact]
        public void Initialize_WhenMapperIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            AppDbContext dbContext = CreateDbContext();
            IMapper mapper = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ReservationService(dbContext, _mockValidator.Object, mapper, _mockLogger.Object));
        }

        [Fact]
        public void Initialize_WhenLoggerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            AppDbContext dbContext = CreateDbContext();
            ILogger<ReservationService> logger = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ReservationService(dbContext, _mockValidator.Object, _mockMapper.Object, logger));
        }

        private AppDbContext CreateDbContext()
        {
            var mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            mockDbContext
                .Setup(m => m.Set<Reservation>())
                .Returns(_mockDbSetReservation.Object);

            return mockDbContext.Object;
        }
    }
}
