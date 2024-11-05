using Application.Activities;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Activity, Activity>();
            CreateMap<Activity, ActivityDto>()
                .ForMember(
                    activityDto => activityDto.HostUserName,
                    option => option.MapFrom(
                        activity => activity.Attendees.FirstOrDefault(activityAttendee => activityAttendee.IsHost).AppUser.UserName
                    )
                );
            CreateMap<ActivityAttendee, Profiles.Profile>()
                .ForMember(
                    profile => profile.DisplayName,
                    opt => opt.MapFrom(activityAttendee => activityAttendee.AppUser.DisplayName)
                )
                .ForMember(
                    profile => profile.Usename,
                    opt => opt.MapFrom(activityAttendee => activityAttendee.AppUser.UserName)
                )
                .ForMember(
                    profile => profile.Bio,
                    opt => opt.MapFrom(activityAttendee => activityAttendee.AppUser.Bio)
                );
        }
    }
}
