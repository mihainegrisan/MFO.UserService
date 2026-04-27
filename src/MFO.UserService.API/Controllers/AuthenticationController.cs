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
    public class AuthenticationRequestBody
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticationRequestBody request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received POST request for user with Email: {Email}", request.Email);

        // Step 1: Validate the credentials (email and password)
        //var user = ValidateUserCredentials(request.Email, request.Password);
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
}