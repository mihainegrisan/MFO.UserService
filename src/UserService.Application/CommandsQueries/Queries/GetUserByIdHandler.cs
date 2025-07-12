using AutoMapper;
using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Application.CommandsQueries.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<GetUserDto?>;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, GetUserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<GetUserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        return user is null 
            ? null 
            : _mapper.Map<GetUserDto>(user);
    }
}