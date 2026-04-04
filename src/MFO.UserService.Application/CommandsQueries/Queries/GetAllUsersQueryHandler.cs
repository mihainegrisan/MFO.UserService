using AutoMapper;
using FluentResults;
using MediatR;
using MFO.Contracts.User.DTOs;
using MFO.UserService.Application.Interfaces;

namespace MFO.UserService.Application.CommandsQueries.Queries;

public sealed record GetAllUsersQuery(int? PageNumber, int? PageSize) : IRequest<Result<IReadOnlyList<UserDto>>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    private const int DefaultPageSize = 3;

    public GetAllUsersQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var pageSize = request.PageSize.GetValueOrDefault(DefaultPageSize);
        var pageNumber = request.PageNumber.HasValue
            ? Math.Max(request.PageNumber.Value - 1, 0)
            : 0;
        
        var users = await _userRepository.GetAllAsync(pageNumber, pageSize, cancellationToken);

        if (users.Count is 0)
        {
            return Result.Ok<IReadOnlyList<UserDto>>(new List<UserDto>());
        }
        
        var usersDto = users
            .Select(user => _mapper.Map<UserDto>(user))
            .ToList();

        return Result.Ok<IReadOnlyList<UserDto>>(usersDto);
    }
}