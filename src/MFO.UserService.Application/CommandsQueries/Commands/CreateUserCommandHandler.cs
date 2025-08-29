using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using MFO.Contracts.User.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Entities;

namespace MFO.UserService.Application.CommandsQueries.Commands;

public sealed record CreateUserCommand(CreateUserDto User) : IRequest<Result<GetUserDto>>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<GetUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly IPasswordHasherService _passwordHasherService;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IValidator<CreateUserCommand> validator,
        IPasswordHasherService passwordHasherService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _validator = validator;
        _passwordHasherService = passwordHasherService;
    }

    public async Task<Result<GetUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(vf => vf.ErrorMessage));
        }

        var userExists = await _userRepository.ExistsByEmailAsync(request.User.Email, cancellationToken);

        if (userExists)
        {
            return Result.Fail($"User with email '{request.User.Email}' already exists.");
        }

        var user = _mapper.Map<User>(request.User);
        user.Id = Guid.NewGuid();
        user.PasswordHash = _passwordHasherService.HashPassword(request.User.Password);
        user.IsActive = true;
        user.CreatedBy = "system";
        user.CreatedDate = DateTime.UtcNow;
        user.LastModifiedBy = "system";
        user.LastModifiedDate = DateTime.UtcNow;
       

        await _userRepository.AddAsync(user, cancellationToken);

        var userDto = _mapper.Map<GetUserDto>(user);

        return Result.Ok(userDto);
    }
}