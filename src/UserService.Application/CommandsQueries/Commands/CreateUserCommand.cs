using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.CommandsQueries.Commands;

public record CreateUserCommand(CreateUserDto User) : IRequest<Result<GetUserDto>>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<GetUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateUserDto> _validator;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IValidator<CreateUserDto> validator)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<GetUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.User, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.ToString());
            // same result
            //return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.User.FirstName,
            LastName = request.User.LastName,
            Email = request.User.Email,
            PasswordHash = request.User.Password, // TODO: In a real application, you should hash the password before storing it
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return _mapper.Map<GetUserDto>(user);

    }
}