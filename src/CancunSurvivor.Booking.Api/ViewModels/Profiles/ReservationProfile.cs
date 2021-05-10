using AutoMapper;
using CancunSurvivor.Booking.Api.Abstractions.Models;
using CancunSurvivor.Booking.Api.ViewModels.Requests;
using CancunSurvivor.Booking.Api.ViewModels.Responses;

namespace CancunSurvivor.Booking.Api.ViewModels.Profiles
{
    /// <summary>
    /// <see cref="Reservation"/> profile.
    /// </summary>
    public class ReservationProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationProfile"/> class.
        /// </summary>
        public ReservationProfile()
        {
            ShouldMapProperty = p => p.GetMethod?.IsPublic == true || p.GetMethod?.IsAssembly == true;

            CreateMap<ReservationCreateRequest, Reservation>()
                .ForMember(x => x.Id, options => options.Ignore())
                .ForMember(x => x.Room, options => options.Ignore());
            CreateMap<ReservationUpdateRequest, Reservation>()
                .ForMember(x => x.Id, options => options.Ignore())
                .ForMember(x => x.CustomerEmail, options => options.Ignore())
                .ForMember(x => x.Room, options => options.Ignore());
            CreateMap<Reservation, ReservationResponse>();
        }
    }
}
