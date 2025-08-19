namespace MFO.UserService.Application.DTOs;

public sealed record CreateUserDto(string FirstName, string LastName, string Email, string Password);
