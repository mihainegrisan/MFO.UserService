using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Globalization;
using MFO.UserService.Application.CommandsQueries.Queries;
using MFO.UserService.Application.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Entities;

namespace MFO.UserService.UnitTests.Application.QueryTests;

[TestFixture]
public class GetUserByEmailQueryHandlerTests
{
    private GetUserByEmailQueryHandler _getUserByEmailQueryHandler;
    private IUserRepository _userRepository;
    private IMapper _mapper;
    private IValidator<GetUserByEmailDto> _validator;

    [SetUp]
    public void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _validator = Substitute.For<IValidator<GetUserByEmailDto>>();

        _getUserByEmailQueryHandler = new GetUserByEmailQueryHandler(_userRepository, _mapper, _validator);
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
            CreatedAt = DateTime.ParseExact("21-05-2025", "dd-MM-yyyy", CultureInfo.InvariantCulture),
            IsActive = true,
            Email = "email@gmail.com"
        };
        var getUserDto = new GetUserDto()
        {
            Id = guid,
            FirstName = "Bob",
            LastName = "Tall",
            CreatedAt = DateTime.ParseExact("21-05-2025", "dd-MM-yyyy", CultureInfo.InvariantCulture),
            IsActive = true,
            Email = "email@gmail.com"
        };

        _userRepository
            .GetByEmailAsync("email@gmail.com", CancellationToken.None)!
            .Returns(Task.FromResult(user));

        _mapper
            .Map<GetUserDto>(Arg.Any<User>())
            .Returns(getUserDto);

        _validator
            .ValidateAsync(Arg.Any<GetUserByEmailDto>(), CancellationToken.None)
            .Returns(Task.FromResult(new ValidationResult()));

        var query = new GetUserByEmailQuery(new GetUserByEmailDto("email@gmail.com"));

        // Act
        var result = await _getUserByEmailQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "Expected success flag to be true");
            Assert.That(result.IsFailed, Is.False, "Expected failure flag to be false");
            Assert.That(result.ValueOrDefault, Is.Not.Null, "Expected non-null Value");

            var dto = result.Value!;
            Assert.That(dto.Id, Is.EqualTo(guid), "Id should match");
            Assert.That(dto.Email, Is.EqualTo("email@gmail.com"), "Email should match");
            Assert.That(dto.FirstName, Is.EqualTo("Bob"), "FirstName should match");
            Assert.That(dto.LastName, Is.EqualTo("Tall"), "LastName should match");
            Assert.That(dto.CreatedAt, Is.EqualTo(user.CreatedAt).Within(TimeSpan.FromSeconds(1)), "CreatedAt should match");
            Assert.That(dto.IsActive, Is.True);
        });

        await _validator.Received(1).ValidateAsync(Arg.Any<GetUserByEmailDto>(), CancellationToken.None);
        await _userRepository.Received(1).GetByEmailAsync(Arg.Any<string>(), CancellationToken.None);
        _mapper.Received(1).Map<GetUserDto>(Arg.Any<User>());
    }

    [Test]
    [TestCase("Email is required.")]
    [TestCase("Invalid email format.")]
    [TestCase("Email must not exceed 100 characters.")]
    [TestCase("Email must be unique.")]
    public async Task Handle_InvalidDto_ReturnsFailure_DoesNotCallRepoOrMapper(string errorMessage)
    {
        // Arrange
        var failures = new[] {
            new ValidationFailure("Email", errorMessage)
        };

        _validator
            .ValidateAsync(Arg.Any<GetUserByEmailDto>(), CancellationToken.None)
            .Returns(Task.FromResult(new ValidationResult(failures)));

        var query = new GetUserByEmailQuery(new GetUserByEmailDto(string.Empty));

        // Act
        var result = await _getUserByEmailQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailed, Is.True, "Expected failure flag to be true");
            Assert.That(result.IsSuccess, Is.False, "Expected success flag to be false");
            Assert.That(result.ValueOrDefault, Is.Null, "Expected null Value");
            Assert.That(result.Errors.Count, Is.EqualTo(1), "Should have exactly one error");
            Assert.That(result.Errors[0].Message, Is.EqualTo(errorMessage), $"Expected error message: '{errorMessage}'");
        });

        await _validator.Received(1).ValidateAsync(Arg.Any<GetUserByEmailDto>(), CancellationToken.None);
        await _userRepository.DidNotReceive().GetByEmailAsync(Arg.Any<string>(), CancellationToken.None);
        _mapper.DidNotReceiveWithAnyArgs().Map<GetUserDto>(null);
    }

    [Test]
    public async Task Handle_UserNotFound_ReturnsFailure_DoesNotCallMapper()
    {
        // Arrange
        _validator
            .ValidateAsync(Arg.Any<GetUserByEmailDto>(), CancellationToken.None)
            .Returns(Task.FromResult(new ValidationResult()));
        _userRepository
            .GetByEmailAsync(Arg.Any<string>(), CancellationToken.None)
            .ReturnsNull();

        var query = new GetUserByEmailQuery(new GetUserByEmailDto("rand@gmail.com"));

        // Act
        var result = await _getUserByEmailQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null, "Expected non-null result");
            Assert.That(result.IsFailed, Is.True, "Expected failure flag to be true");
            Assert.That(result.IsSuccess, Is.False, "Expected success flag to be false");
            Assert.That(result.ValueOrDefault, Is.Null, "Expected null Value");
            Assert.That(result.Errors.Count, Is.EqualTo(1), "Should have exactly one error");
            Assert.That(result.Errors[0].Message, Is.EqualTo("User not found"), "Expected error message: 'User not found'.");
        });

        await _validator.Received(1).ValidateAsync(Arg.Any<GetUserByEmailDto>(), CancellationToken.None);
        await _userRepository.Received(1).GetByEmailAsync(Arg.Any<string>(), CancellationToken.None);
        _mapper.DidNotReceiveWithAnyArgs().Map<GetUserDto>(null);
        // Mapper.DidNotReceive().Map<GetUserDto>(Arg.Any<User>());
    }
}
