using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.CommandsQueries.Commands;
using UserService.Application.CommandsQueries.Queries;
using UserService.Application.DTOs;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IMediator _mediator;

    public UsersController(
        ILogger<UsersController> logger,
        IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);

        return result is not null 
            ? Ok(result) 
            : NotFound($"User with ID {id} not found.");
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateUserDto user, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateUserCommand(user), cancellationToken);

        return Ok(result);
    }
}