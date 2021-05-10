using AutoMapper;
using CancunSurvivor.Booking.Api.ViewModels.Profiles;
using Xunit;

namespace CancunSurvivor.Booking.Api.UnitTest.ViewModels.Profiles
{
    public class ReservationProfileTests
    {
        [Fact]
        public void Profile_ShouldMapSuccessfully()
        {
            // Arrange
            var configuration = new MapperConfiguration(config => config.AddProfile<ReservationProfile>());

            // Act

            // Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
