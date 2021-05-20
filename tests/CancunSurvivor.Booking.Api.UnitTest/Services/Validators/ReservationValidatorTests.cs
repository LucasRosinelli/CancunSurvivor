using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CancunSurvivor.Booking.Api.Abstractions.Models;
using CancunSurvivor.Booking.Api.Services.Validators;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CancunSurvivor.Booking.Api.UnitTest.Services.Validators
{
    public class ReservationValidatorTests
    {
        private static readonly Random Rnd = new Random();
        private readonly Mock<ILogger<ReservationValidator>> _mockLogger;
        private readonly ReservationValidator _validator;

        public ReservationValidatorTests()
        {
            _mockLogger = new Mock<ILogger<ReservationValidator>>();
            _validator = new ReservationValidator(_mockLogger.Object);
        }

        [Theory]
        [MemberData(nameof(InitInvalidCustomerEmail))]
        public void Reservation_WhenInvalidCustomerEmail_ShouldHaveFailure(string customerEmail, bool expectedRequiredFailure, bool expectedMaxLengthFailure, bool expectedInvalidEmailAddressFailure)
        {
            // Arrange
            string expectedRequiredFailureMessage = "The CustomerEmail field is required";
            string expectedMaxLengthFailureMessage = "The max length allowed for the CustomerEmail is 500";
            string expectedInvalidEmailAddressFailureMessage = "The CustomerEmail must be a valid email";
            var reservation = new Reservation
            {
                CustomerEmail = customerEmail,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(2),
            };

            // Act
            IList<ValidationFailure> failures = _validator.ShouldHaveValidationErrorFor(r => r.CustomerEmail, reservation).ToList();

            // Assert
            Assert.NotNull(failures);
            Assert.NotEmpty(failures);
            Assert.Equal(expectedRequiredFailure, failures.Any(x => x.ErrorMessage == expectedRequiredFailureMessage));
            Times expectedRequiredFailureLogEntry = expectedRequiredFailure ? Times.Once() : Times.Never();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(expectedRequiredFailureMessage)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                expectedRequiredFailureLogEntry);
            Assert.Equal(expectedMaxLengthFailure, failures.Any(x => x.ErrorMessage == expectedMaxLengthFailureMessage));
            Times expectedMaxLengthFailureLogEntry = expectedMaxLengthFailure ? Times.Once() : Times.Never();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(expectedMaxLengthFailureMessage)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                expectedMaxLengthFailureLogEntry);
            Assert.Equal(expectedInvalidEmailAddressFailure, failures.Any(x => x.ErrorMessage == expectedInvalidEmailAddressFailureMessage));
            Times expectedInvalidEmailAddressFailureLogEntry = expectedInvalidEmailAddressFailure ? Times.Once() : Times.Never();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(expectedInvalidEmailAddressFailureMessage)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                expectedInvalidEmailAddressFailureLogEntry);
        }

        private static IEnumerable<object[]> InitInvalidCustomerEmail()
        {
            var result = new List<object[]>
            {
                new object[] { null!, true, false, false, },
                new object[] { "", true, false, true, },
                new object[] { "   ", true, false, true, },
            };

            string emailBeginning = "too_long_email-";
            string emailEnding = "@some-domain.com";

            result.Add(GenerateInvalidEmailEntry("1", emailBeginning, emailEnding));
            result.Add(GenerateInvalidEmailEntry("2", emailBeginning, emailEnding));

            return result;
        }

        private static object[] GenerateInvalidEmailEntry(string identifier, string emailBeginning, string emailEnding)
        {
            int randomEmailPartToAppendLength = 501 - identifier.Length - emailBeginning.Length - emailEnding.Length;

            string randomContent = GenerateRandomLetterDigitString(randomEmailPartToAppendLength);
            return new object[]
            {
                $"{identifier}{emailBeginning}{randomContent}{emailEnding}",
                false,
                true,
                false,
            };
        }

        private static string GenerateRandomLetterDigitString(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randomArray = Enumerable.Repeat(validChars, length).Select(s => s[Rnd.Next(s.Length)]).ToArray();
            var randomContent = new StringBuilder(length);
            randomContent.Append(randomArray);

            return randomContent.ToString();
        }
    }
}
