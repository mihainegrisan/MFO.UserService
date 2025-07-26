using System.Collections.Immutable;
using AutoMapper;
using FluentResults;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Application.CommandsQueries.Queries;

public sealed record GetAllUsersQuery(int? PageNumber, int? PageSize) : IRequest<Result<IReadOnlyList<GetUserDto>>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<GetUserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository)); ;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
    }

    public async Task<Result<IReadOnlyList<GetUserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var pageSize = request.PageSize.GetValueOrDefault(3);
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