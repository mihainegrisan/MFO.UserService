using AutoMapper;
using FluentResults;
using MediatR;
using MFO.Contracts.User.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Entities;

namespace MFO.UserService.Application.CommandsQueries.Commands;

public sealed record CreateUserCommand(CreateUserDto User) : IRequest<Result<UserDto>>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasherService _passwordHasherService;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IPasswordHasherService passwordHasherService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasherService = passwordHasherService;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.ExistsByEmailAsync(request.User.Email, cancellationToken);
        if (userExists)
        {
            return Result.Fail($"User with email '{request.User.Email}' already exists.");
        }

        var user = _mapper.Map<User>(request.User);
        user.UserId = Guid.CreateVersion7();
        user.PasswordHash = _passwordHasherService.HashPassword(request.User.Password);
        user.IsActive = true;
        user.CreatedBy = "system";
        user.CreatedDate = DateTime.UtcNow;
        user.LastModifiedBy = "system";
        user.LastModifiedDate = DateTime.UtcNow;
        
        await _userRepository.AddAsync(user, cancellationToken);

        var userDto = _mapper.Map<UserDto>(user);

        return Result.Ok(userDto);
    }
}