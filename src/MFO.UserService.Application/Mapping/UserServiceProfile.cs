using AutoMapper;

namespace MFO.UserService.Application.Mapping;

public class UserServiceProfile : Profile
{
    // This class is intended to be used with AutoMapper or similar libraries
    // to define mappings between domain models and DTOs.
    // You can implement mapping configurations here.

    public UserServiceProfile()
    {
        // Define your mappings here
        // CreateMap<SourceType, DestinationType>();

        CreateMap<MFO.UserService.Domain.Entities.User, MFO.Contracts.User.DTOs.GetUserDto>();
    }
}