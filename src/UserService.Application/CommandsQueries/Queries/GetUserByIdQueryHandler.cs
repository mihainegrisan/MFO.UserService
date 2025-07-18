using AutoMapper;
using FluentResults;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Application.CommandsQueries.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<GetUserDto>>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<GetUserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Result.Fail("User not found");
        }

        var userDto = _mapper.Map<GetUserDto>(user);
        return Result.Ok(userDto);
    }
}