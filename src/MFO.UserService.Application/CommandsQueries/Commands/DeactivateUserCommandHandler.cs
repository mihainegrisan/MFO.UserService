using AutoMapper;
using FluentResults;
using MediatR;
using MFO.UserService.Application.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Errors;

namespace MFO.UserService.Application.CommandsQueries.Commands;

public record DeactivateUserCommand(Guid Id) : IRequest<Result<GetUserDto>>;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result<GetUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public DeactivateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetUserDto>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Result.Fail<GetUserDto>(new NotFoundError($"User with ID '{request.Id}' not found."));
        }

        var deactivated = await _userRepository.SetUserActiveStateAsync(user, false, cancellationToken);

        if (!deactivated)
        {
            return Result.Fail($"Failed to deactivate user with ID '{request.Id}'.");
        }

        var userDto = _mapper.Map<GetUserDto>(deactivated);

        return Result.Ok(userDto);
    }
}