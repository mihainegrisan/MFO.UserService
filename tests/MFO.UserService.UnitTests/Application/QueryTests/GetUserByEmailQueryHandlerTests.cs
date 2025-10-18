using AutoMapper;
using FluentValidation;
using MFO.Contracts.User.DTOs;
using MFO.UserService.Application.CommandsQueries.Queries;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Globalization;

namespace MFO.UserService.UnitTests.Application.QueryTests;

[TestFixture]
public class GetUserByEmailQueryHandlerTests
{
    private GetUserByEmailQueryHandler _getUserByEmailQueryHandler;
    private IUserRepository _userRepository;
    private IMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();

        _getUserByEmailQueryHandler = new GetUserByEmailQueryHandler(_userRepository, _mapper);
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsMappedDto()
    {
        // Arrange
        var guid = new Guid("e005a010-c116-42cf-ac78-01b8290d2a2d");
        var user = new User
        {
            Id = guid,
            FirstName = "Bob",
            LastName = "Tall",
            IsActive = true,
            Email = "email@gmail.com",
            CreatedDate = DateTime.ParseExact("21-05-2025", "dd-MM-yyyy", CultureInfo.InvariantCulture),
        };
        var getUserDto = new GetUserDto()
        {
            Id = guid,
            FirstName = "Bob",
            LastName = "Tall",
            IsActive = true,
            Email = "email@gmail.com",
            CreatedAt = DateTime.ParseExact("21-05-2025", "dd-MM-yyyy", CultureInfo.InvariantCulture),
        };

        _userRepository
            .GetByEmailAsync("email@gmail.com", CancellationToken.None)!
            .Returns(Task.FromResult(user));

        _mapper
            .Map<GetUserDto>(Arg.Any<User>())
            .Returns(getUserDto);

        var query = new GetUserByEmailQuery(new GetUserByEmailDto("email@gmail.com"));

        // Act
        var result = await _getUserByEmailQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True, "Expected success flag to be true");
            Assert.That(result.IsFailed, Is.False, "Expected failure flag to be false");
            Assert.That(result.ValueOrDefault, Is.Not.Null, "Expected non-null Value");

            var dto = result.Value!;
            Assert.That(dto.Id, Is.EqualTo(guid), "Id should match");
            Assert.That(dto.Email, Is.EqualTo("email@gmail.com"), "Email should match");
            Assert.That(dto.FirstName, Is.EqualTo("Bob"), "FirstName should match");
            Assert.That(dto.LastName, Is.EqualTo("Tall"), "LastName should match");
            Assert.That(dto.CreatedAt, Is.EqualTo(user.CreatedDate).Within(TimeSpan.FromSeconds(1)), "CreatedAt should match");
            Assert.That(dto.IsActive, Is.True);
        }

        await _userRepository.Received(1).GetByEmailAsync(Arg.Any<string>(), CancellationToken.None);
        _mapper.Received(1).Map<GetUserDto>(Arg.Any<User>());
    }

    [Test]
    public async Task Handle_InvalidDto_ReturnsFailure_DoesNotCallMapper()
    {
        // Arrange
        var query = new GetUserByEmailQuery(new GetUserByEmailDto(string.Empty));

        // Act
        var result = await _getUserByEmailQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsFailed, Is.True, "Expected failure flag to be true");
            Assert.That(result.IsSuccess, Is.False, "Expected success flag to be false");
            Assert.That(result.ValueOrDefault, Is.Null, "Expected null Value");
            Assert.That(result.Errors.Count, Is.EqualTo(1), "Should have exactly one error");
            Assert.That(result.Errors[0].Message, Is.EqualTo("User with Email '' not found."));
        }

        await _userRepository.Received(1).GetByEmailAsync(Arg.Any<string>(), CancellationToken.None);
        _mapper.DidNotReceiveWithAnyArgs().Map<GetUserDto>(null);
    }

    [Test]
    public async Task Handle_UserNotFound_ReturnsFailure_DoesNotCallMapper()
    {
        // Arrange
        _userRepository
            .GetByEmailAsync(Arg.Any<string>(), CancellationToken.None)
            .ReturnsNull();

        var query = new GetUserByEmailQuery(new GetUserByEmailDto("rand@gmail.com"));

        // Act
        var result = await _getUserByEmailQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null, "Expected non-null result");
            Assert.That(result.IsFailed, Is.True, "Expected failure flag to be true");
            Assert.That(result.IsSuccess, Is.False, "Expected success flag to be false");
            Assert.That(result.ValueOrDefault, Is.Null, "Expected null Value");
            Assert.That(result.Errors.Count, Is.EqualTo(1), "Should have exactly one error");
            Assert.That(result.Errors[0].Message, Is.EqualTo("User with Email 'rand@gmail.com' not found."));
        }

        await _userRepository.Received(1).GetByEmailAsync(Arg.Any<string>(), CancellationToken.None);
        _mapper.DidNotReceiveWithAnyArgs().Map<GetUserDto>(null);
        // Mapper.DidNotReceive().Map<GetUserDto>(Arg.Any<User>());
    }
}
