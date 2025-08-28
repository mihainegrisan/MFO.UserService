using AutoMapper;
using MFO.Contracts.User.DTOs;
using MFO.UserService.Domain.Entities;

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

        // Entity → DTO
        CreateMap<User, GetUserDto>();

        // DTO → Entity(for create / update)
        CreateMap<CreateUserDto, User>();
    }
}