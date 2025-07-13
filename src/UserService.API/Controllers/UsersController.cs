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
        _logger.LogInformation("Received GET request for user with Id: {UserId}", id);

        var result = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);

        if (result.IsFailed)
        {
            _logger.LogWarning("User with Id: {UserId} not found. Errors: {@Errors}", id, result.Errors);

            return NotFound(result.Errors);
            // return NotFound(result.Errors.FirstOrDefault()?.Message ?? "User not found");
        }

        _logger.LogInformation("User with Id: {UserId} retrieved successfully.", id);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateUserDto user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received POST request to create user: {@User}", user);

        var result = await _mediator.Send(new CreateUserCommand(user), cancellationToken);

        if (result.IsFailed)
        {
            _logger.LogWarning("Failed to create user. Errors: {@Errors}", result.Errors);
            return BadRequest(result.Errors);
        }

        var createdUser = result.Value;

        _logger.LogInformation("User created successfully with Id: {UserId}", createdUser.Id);

        return CreatedAtAction(
            nameof(Get),
            new { id = createdUser.Id },
            createdUser
        );
    }
}