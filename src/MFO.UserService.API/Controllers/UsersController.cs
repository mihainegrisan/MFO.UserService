using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MFO.UserService.Application.CommandsQueries.Commands;
using MFO.UserService.Application.CommandsQueries.Queries;
using MFO.UserService.Application.DTOs;
using MFO.UserService.Application.Utilities;
using MFO.UserService.Domain.Errors;

namespace MFO.UserService.API.Controllers;

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

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A user</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/users/3bda226a-d2fc-477f-a545-7b4dd45df670
    ///
    /// </remarks>
    /// <response code="200">Returns the user with the corresponding id</response>
    /// <response code="404">If the user wasn't found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<IError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received GET request for user with Id: {UserId}", id);

        var result = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);

        if (result.IsFailed)
        {
            _logger.LogWarning("User with Id: {UserId} not found. Errors: {@Errors}", id, result.Errors);

            if (result.HasError<NotFoundError>())
            {
                return NotFound();
            }

            return BadRequest(result.Errors);
        }

        _logger.LogInformation("User with Id: {UserId} retrieved successfully.", id);

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a user by their email.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A user</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST api/users
    ///
    /// </remarks>
    /// <response code="200">Returns the user with the corresponding email</response>
    /// <response code="404">If the user wasn't found</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<IError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByEmail([FromBody] GetUserByEmailDto user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received POST request to search user by Email: {UserEmail}", user.Email);

        var result = await _mediator.Send(new GetUserByEmailQuery(user), cancellationToken);

        if (result.IsFailed)
        {
            _logger.LogWarning("User with Email: {UserEmail} not found. Errors: {@Errors}", user.Email, result.Errors);
            
            if (result.HasError<NotFoundError>())
            {
                return NotFound();
            }

            return BadRequest(result.Errors);
        }

        _logger.LogInformation("User with Email: {UserEmail} retrieved successfully.", user.Email);

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The number of items on the page</param>
    /// <param name="cancellationToken"></param>
    /// <returns>All users</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET api/users
    ///     GET api/users?pageNumber=2
    ///     GET api/users?pageNumber=2&pageSize=10
    /// 
    /// </remarks>
    /// <response code="200">Returns all users</response>
    /// <response code="404">If no users were found</response>
    [HttpGet]
    [OutputCache(PolicyName = CachePolicies.GetAll)]
    [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<IError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllUsers([FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received GET request for all users.");

        // Can create a factory to get the correct Query to send if I decide to make multiple
        var result = await _mediator.Send(new GetAllUsersQuery(pageNumber, pageSize), cancellationToken);

        if (result.IsFailed)
        {
            _logger.LogWarning("Failed to retrieve users. Errors: {@Errors}", result.Errors);
            
            if (result.HasError<NotFoundError>())
            {
                return NotFound();
            }

            return BadRequest(result.Errors);
        }

        _logger.LogInformation("Retrieved {UserCount} users successfully.", result.Value.Count);

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a User.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A newly created User</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST api/users
    ///     {
    ///        "firstName": "Mihai",
    ///        "lastName": "N",
    ///        "email": "random1@gmail.com",
    ///        "password": "test1234"
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Returns the newly created user</response>
    /// <response code="400">If it fails to create the user due to validation errors</response>
    [HttpPost]
    [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(List<IError>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto user, CancellationToken cancellationToken)
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
            nameof(GetUserById),
            new { id = createdUser.Id },
            createdUser
        );
    }

    /// <summary>
    /// Updates a User.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated User</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT api/users/3bda226a-d2fc-477f-a545-7b4dd45df670
    ///     {
    ///        "firstName": "Mihai",
    ///        "lastName": "N",
    ///        "email": "random1@gmail.com",
    ///        "password": "test1234",
    ///        "isActive": true
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Returns the updated user</response>
    /// <response code="400">If it fails to update the user due to validation errors</response>
    /// <response code="404">If no user is found</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<IError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received PUT request to update user: {@User}", user);

        if (id != user.Id)
        {
            return BadRequest("Mismatched user ID.");
        }

        var result = await _mediator.Send(new UpdateUserCommand(user), cancellationToken);

        if (result.IsFailed)
        {
            _logger.LogWarning("Failed to update user. Errors: {@Errors}", result.Errors);

            if (result.HasError<NotFoundError>()) // && result.Errors.Any(e => e.Message.Contains("User not found."))
            {
                return NotFound();
            }

            return BadRequest(result.Errors);
        }

        var updatedUser = result.Value;

        _logger.LogInformation("User with Id: {UserId} was updated successfully.", updatedUser.Id);

        return Ok(updatedUser);
    }

    /// <summary>
    /// Deactivates a User (soft delete).
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The deactivated User</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE api/users/deactivate/3bda226a-d2fc-477f-a545-7b4dd45df670
    ///     
    /// </remarks>
    /// <response code="200">Returns the deactivated user</response>
    /// <response code="400">If it fails to deactivate the user due to other errors</response>
    /// <response code="404">If no user is found</response>
    [HttpDelete("deactivate/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received DELETE request to deactivate user: {UserId}", id);

        var result = await _mediator.Send(new DeactivateUserCommand(id), cancellationToken);

        if (result.IsFailed)
        {
            _logger.LogWarning("Failed to deactivate user. Errors: {@Errors}", result.Errors);

            if (result.HasError<NotFoundError>())
            {
                return NotFound(result.Errors);
            }

            return BadRequest(result.Errors);
        }

        var deactivatedUser = result.Value;

        _logger.LogInformation("User with Id: {UserId} was deactivated successfully.", deactivatedUser.Id);

        return Ok(deactivatedUser);
    }

    /// <summary>
    /// Deletes a User (hard).
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Nothing</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE api/users/3bda226a-d2fc-477f-a545-7b4dd45df670
    ///     
    /// </remarks>
    /// <response code="200">Returns ok</response>
    /// <response code="400">If it fails to delete the user due to other errors</response>
    /// <response code="404">If no user is found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HardDeleteUser(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received DELETE request to delete user: {UserId}", id);

        var result = await _mediator.Send(new DeleteUserCommand(id), cancellationToken);

        if (result.IsFailed)
        {
            _logger.LogWarning("Failed to delete user. Errors: {@Errors}", result.Errors);

            if (result.HasError<NotFoundError>())
            {
                return NotFound(result.Errors);
            }

            return BadRequest(result.Errors);
        }

        _logger.LogInformation("User with Id: {UserId} was deleted successfully.", id);

        return Ok();
    }
}