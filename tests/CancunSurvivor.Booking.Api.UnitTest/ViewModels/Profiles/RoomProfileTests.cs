using AutoMapper;
using CancunSurvivor.Booking.Api.ViewModels.Profiles;
using Xunit;

namespace CancunSurvivor.Booking.Api.UnitTest.ViewModels.Profiles
{
    public class RoomProfileTests
    {
        [Fact]
        public void Profile_ShouldMapSuccessfully()
        {
            // Arrange
            var configuration = new MapperConfiguration(config => config.AddProfile<RoomProfile>());

            // Act

            // Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
