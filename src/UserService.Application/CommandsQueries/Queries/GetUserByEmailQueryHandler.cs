using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Application.CommandsQueries.Queries;

public sealed record GetUserByEmailQuery(GetUserByEmailDto User) : IRequest<Result<GetUserDto>>;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Result<GetUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetUserByEmailDto> _validator;

    public GetUserByEmailQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IValidator<GetUserByEmailDto> validator)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository)); ;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
        _validator = validator ?? throw new ArgumentNullException(nameof(validator)); ;
    }

    public async Task<Result<GetUserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.User, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(vf => vf.ErrorMessage));
        }

        var user = await _userRepository.GetByEmailAsync(request.User.Email, cancellationToken);

        if (user is null)
        {
            return Result.Fail("User not found");
        }

        var userDto = _mapper.Map<GetUserDto>(user);

        return Result.Ok(userDto);
    }
}