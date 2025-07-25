using AutoMapper;
using FluentResults;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Application.CommandsQueries.Queries;

public sealed record GetAllUsersQuery(int? PageNumber, int? PageSize) : IRequest<Result<List<GetUserDto>>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<GetUserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<GetUserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
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

        return Result.Ok(usersDto);
    }
}