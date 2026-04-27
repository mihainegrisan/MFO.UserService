using MediatR;
using MFO.UserService.Application.CommandsQueries.Commands;
using MFO.UserService.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace MFO.UserService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private IMediator _mediator;
    private ILogger<AuthenticationController> _logger;

    public AuthenticationController(IMediator mediator, ILogger<AuthenticationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // Won't use this outside of this class, so I'm scoping it to this namespace
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received POST request for user with Email: {Email}", request.Email);

        var result = await _mediator.Send(new AuthenticateUserCommand(request.Email, request.Password), cancellationToken);

        if (result.IsFailed)
        {
            if (result.HasError<NotFoundError>())
            {
                _logger.LogInformation("User with Email: {Email} not found.", request.Email);

                return NotFound();
            }

            if (result.HasError<UnauthorizedAccessError>())
            {
                _logger.LogInformation("Unauthorized");

                return Unauthorized();
            }

            _logger.LogWarning("Failed to authenticate user with Email: {Email}. Errors: {@Errors}", request.Email, result.Errors);

            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    public class RefreshRequest
    {
        public required string RefreshToken { get; set; }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received POST request for RefreshToken rotation.");

        var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken), cancellationToken);

        if (result.IsFailed)
        {
            if (result.HasError<UnauthorizedAccessError>())
            {
                _logger.LogInformation("Unauthorized");

                return Unauthorized();
            }

            _logger.LogWarning("Failed to rotate RefreshToken. Errors: {@Errors}", result.Errors);

            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }
}