using AutoMapper;
using FluentResults;
using MediatR;
using MFO.Contracts.User.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Errors;

namespace MFO.UserService.Application.CommandsQueries.Queries;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Result.Fail(new NotFoundError($"User with ID '{request.Id}' not found."));
        }

        var userDto = _mapper.Map<UserDto>(user);
        return Result.Ok(userDto);
    }
}