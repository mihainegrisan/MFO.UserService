using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using MFO.Contracts.User.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Errors;

namespace MFO.UserService.Application.CommandsQueries.Queries;

public sealed record GetUserByEmailQuery(GetUserByEmailDto User) : IRequest<Result<GetUserDto>>;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Result<GetUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByEmailQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetUserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.User.Email, cancellationToken);
        if (user is null)
        {
            return Result.Fail(new NotFoundError($"User with Email '{request.User.Email}' not found."));
        }

        var userDto = _mapper.Map<GetUserDto>(user);

        return Result.Ok(userDto);
    }
}