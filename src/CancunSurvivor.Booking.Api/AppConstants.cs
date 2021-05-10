using System;

namespace CancunSurvivor.Booking.Api
{
    public static class AppConstants
    {
        private static Guid _defaultRoomId = Guid.NewGuid();

        public static Guid DefaultRoomId
        {
            get => _defaultRoomId;
        }
    }
}
