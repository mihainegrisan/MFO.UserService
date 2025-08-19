using AutoMapper;
using FluentResults;
using MediatR;
using MFO.UserService.Application.DTOs;
using MFO.UserService.Application.Interfaces;

namespace MFO.UserService.Application.CommandsQueries.Queries;

public sealed record GetAllUsersQuery(int? PageNumber, int? PageSize) : IRequest<Result<IReadOnlyList<GetUserDto>>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<GetUserDto>>>
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

    public async Task<Result<IReadOnlyList<GetUserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var pageSize = request.PageSize.GetValueOrDefault(DefaultPageSize);
        var pageNumber = request.PageNumber.HasValue
            ? Math.Max(request.PageNumber.Value - 1, 0)
            : 0;
        
        var users = await _userRepository.GetAllAsync(pageNumber, pageSize, cancellationToken);

        if (!users.Any())
        {
            return Result.Fail("No users found.");
        }
        
        var usersDto = users
            .Select(user => _mapper.Map<GetUserDto>(user))
            .ToList();

        return Result.Ok<IReadOnlyList<GetUserDto>>(usersDto);
    }
}