using AutoMapper;
using FluentResults;
using MediatR;
using MFO.Contracts.User.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Errors;

namespace MFO.UserService.Application.CommandsQueries.Commands;

public sealed record UpdateUserCommand(UpdateUserDto User) : IRequest<Result<UserDto>>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasherService _passwordHasherService;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IPasswordHasherService passwordHasherService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasherService = passwordHasherService;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByIdAsync(request.User.UserId, cancellationToken);
        if (existingUser is null)
        {
            return Result.Fail(new NotFoundError($"User with ID '{request.User.UserId}' not found."));
        }

        existingUser.FirstName = request.User.FirstName;
        existingUser.LastName = request.User.LastName;
        existingUser.Email = request.User.Email;

        if (request.User.Password != null)
        {
            existingUser.PasswordHash = _passwordHasherService.HashPassword(request.User.Password);
        }
        
        if (request.User.IsActive.HasValue)
        {
            existingUser.IsActive = request.User.IsActive.Value;
        }
        
        existingUser.LastModifiedBy = "system";
        existingUser.LastModifiedDate = DateTime.UtcNow;

        var updatedUser = await _userRepository.UpdateAsync(existingUser, cancellationToken);

        var userDto = _mapper.Map<UserDto>(updatedUser);

        return Result.Ok(userDto);
    }
}