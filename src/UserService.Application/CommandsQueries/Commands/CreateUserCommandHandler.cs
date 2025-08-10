using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.CommandsQueries.Commands;

public sealed record CreateUserCommand(CreateUserDto User) : IRequest<Result<GetUserDto>>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<GetUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateUserDto> _validator;
    private readonly IPasswordHasherService _passwordHasherService;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IValidator<CreateUserDto> validator,
        IPasswordHasherService passwordHasherService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _validator = validator;
        _passwordHasherService = passwordHasherService;
    }

    public async Task<Result<GetUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.User, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(vf => vf.ErrorMessage));
        }

        var userExists = await _userRepository.ExistsByEmailAsync(request.User.Email, cancellationToken);

        if (userExists)
        {
            return Result.Fail($"User with email '{request.User.Email}' already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.User.FirstName,
            LastName = request.User.LastName,
            Email = request.User.Email,
            PasswordHash = _passwordHasherService.HashPassword(request.User.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(user, cancellationToken);

        var userDto = _mapper.Map<GetUserDto>(user);

        return Result.Ok(userDto);
    }
}