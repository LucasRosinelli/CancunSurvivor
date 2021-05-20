using System;

namespace CancunSurvivor.Booking.Api
{
    /// <summary>
    /// Represents shared values in the entire application.
    /// </summary>
    public static class AppConstants
    {
        private static Guid _defaultRoomId = Guid.NewGuid();

        /// <summary>
        /// The default room id.
        /// </summary>
        public static Guid DefaultRoomId
        {
            get => _defaultRoomId;
        }
    }
}
