using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Errors;

namespace UserService.Application.CommandsQueries.Commands;

public sealed record UpdateUserCommand(UpdateUserDto User) : IRequest<Result<GetUserDto>>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<GetUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateUserDto> _validator;
    private readonly IPasswordHasherService _passwordHasherService;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IValidator<UpdateUserDto> validator,
        IPasswordHasherService passwordHasherService)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository)); ;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
        _passwordHasherService = passwordHasherService ?? throw new ArgumentNullException(nameof(passwordHasherService));
    }

    public async Task<Result<GetUserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.User, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(vf => vf.ErrorMessage));
        }

        var existingUser = await _userRepository.GetByIdAsync(request.User.Id, cancellationToken);
        if (existingUser is null)
        {
            return Result.Fail(new NotFoundError($"User with ID '{request.User.Id}' not found."));
        }

        existingUser.FirstName = request.User.FirstName;
        existingUser.LastName = request.User.LastName;
        existingUser.Email = request.User.Email;
        existingUser.PasswordHash = _passwordHasherService.HashPassword(request.User.Password);
        existingUser.IsActive = request.User.IsActive;

        var updatedUser = await _userRepository.UpdateAsync(existingUser, cancellationToken);

        var userDto = _mapper.Map<GetUserDto>(updatedUser);

        return Result.Ok(userDto);
    }
}