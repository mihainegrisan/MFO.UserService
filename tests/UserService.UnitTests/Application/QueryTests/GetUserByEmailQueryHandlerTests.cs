using FluentValidation.Results;
using NSubstitute;
using System.Globalization;
using UserService.Application.CommandsQueries.Queries;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.UnitTests.TestHelpers;

namespace UserService.UnitTests.Application.QueryTests;

[TestFixture]
public class GetUserByEmailQueryHandlerTests : TestBase
{
    private GetUserByEmailQueryHandler _getUserByEmailQueryHandler;

    [SetUp]
    public override void Setup()
    {
        base.Setup();

        _getUserByEmailQueryHandler = new GetUserByEmailQueryHandler(UserRepository, Mapper, Validator);
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

        UserRepository
            .GetByEmailAsync("email@gmail.com", CancellationToken.None)!
            .Returns(Task.FromResult(user));

        Mapper
            .Map<GetUserDto>(Arg.Any<User>())
            .Returns(getUserDto);

        Validator
            .ValidateAsync(Arg.Any<GetUserByEmailDto>(), CancellationToken.None)
            .Returns(Task.FromResult(new ValidationResult()));

        var query = new GetUserByEmailQuery(new GetUserByEmailDto { Email = "email@gmail.com" });

        // Act
        var result = await _getUserByEmailQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "Expected success flag");
            Assert.That(result.Value, Is.Not.Null, "Expected non-null Value");

            var dto = result.Value!;
            Assert.That(dto.Id, Is.EqualTo(guid), "Id should match");
            Assert.That(dto.Email, Is.EqualTo("email@gmail.com"), "Email should match");
            Assert.That(dto.FirstName, Is.EqualTo("Bob"), "FirstName should match");
            Assert.That(dto.LastName, Is.EqualTo("Tall"), "LastName should match");
            Assert.That(dto.CreatedAt, Is.EqualTo(user.CreatedAt).Within(TimeSpan.FromSeconds(1)), "CreatedAt should match");
            Assert.That(dto.IsActive, Is.True);
        });

        await Validator.Received(1).ValidateAsync(Arg.Any<GetUserByEmailDto>(), CancellationToken.None);
        await UserRepository.Received(1).GetByEmailAsync(Arg.Any<string>(), CancellationToken.None);
        Mapper.Received(1).Map<GetUserDto>(Arg.Any<User>());
    }
}
