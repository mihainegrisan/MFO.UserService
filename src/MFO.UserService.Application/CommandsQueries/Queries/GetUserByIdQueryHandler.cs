using AutoMapper;
using FluentResults;
using MediatR;
using MFO.Contracts.User;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Errors;

namespace MFO.UserService.Application.CommandsQueries.Queries;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<Result<GetUserDto>>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserDto>>
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

    public async Task<Result<GetUserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Result.Fail(new NotFoundError($"User with ID '{request.Id}' not found."));
        }

        var userDto = _mapper.Map<GetUserDto>(user);
        return Result.Ok(userDto);
    }
}