namespace MFO.UserService.Application.DTOs;

public sealed record UpdateUserDto(Guid Id, string FirstName, string LastName, string Email, string Password, bool IsActive);