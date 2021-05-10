using AutoMapper;
using CancunSurvivor.Booking.Api.Abstractions.Models;
using CancunSurvivor.Booking.Api.ViewModels.Requests;
using CancunSurvivor.Booking.Api.ViewModels.Responses;

namespace CancunSurvivor.Booking.Api.ViewModels.Profiles
{
    /// <summary>
    /// <see cref="Room"/> profile.
    /// </summary>
    public class RoomProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomProfile"/> class.
        /// </summary>
        public RoomProfile()
        {
            CreateMap<RoomUpdateRequest, Room>()
                .ForMember(x => x.Id, options => options.Ignore())
                .ForMember(x => x.Reservations, options => options.Ignore());
            CreateMap<Room, RoomResponse>();
        }
    }
}
