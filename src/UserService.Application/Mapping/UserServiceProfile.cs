using AutoMapper;

namespace UserService.Application.Mapping;

public class UserServiceProfile : Profile
{
    // This class is intended to be used with AutoMapper or similar libraries
    // to define mappings between domain models and DTOs.
    // You can implement mapping configurations here.

    public UserServiceProfile()
    {
        // Define your mappings here
        // CreateMap<SourceType, DestinationType>();
        // Example:
        // CreateMap<User, UserDto>()
        //     .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));

        CreateMap<Domain.Entities.User, Application.DTOs.UserDto>();
    }
}