using AutoMapper;
using NSubstitute;
using System.Globalization;
using MFO.UserService.Application.CommandsQueries.Queries;
using MFO.UserService.Application.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Entities;

namespace MFO.UserService.UnitTests.Application.QueryTests;

[TestFixture]
public class GetAllUsersQueryHandlerTests
{
    private IUserRepository _userRepository;
    private IMapper _mapper;
    private GetAllUsersQueryHandler _getAllUsersQueryHandler;

    [SetUp]
    public void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();

        _getAllUsersQueryHandler = new GetAllUsersQueryHandler(_userRepository, _mapper);
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsAllRequestedUsers()
    {
        var guid1 = new Guid("e005a010-c116-42cf-ac78-01b8290d2bbb");
        var guid2 = new Guid("e005a010-c116-42cf-ac78-01b8290d2aaa");

        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = guid1,
                FirstName = "Bob",
                LastName = "Tall",
                CreatedAt = DateTime.ParseExact("21-05-2025", "dd-MM-yyyy", CultureInfo.InvariantCulture),
                IsActive = true,
                Email = "bob@gmail.com"
            },
            new()
            {
                Id = guid2,
                FirstName = "Colt",
                LastName = "Small",
                CreatedAt = DateTime.ParseExact("05-02-2024", "dd-MM-yyyy", CultureInfo.InvariantCulture),
                IsActive = true,
                Email = "colt@gmail.com"
            }
        };

        var getUserDtos = new List<GetUserDto>
        {
            new()
            {
                Id = new Guid("e005a010-c116-42cf-ac78-01b8290d2bbb"),
                FirstName = "Bob",
                LastName = "Tall",
                CreatedAt = DateTime.ParseExact("21-05-2025", "dd-MM-yyyy", CultureInfo.InvariantCulture),
                IsActive = true,
                Email = "bob@gmail.com"
            },
            new()
            {
                Id = new Guid("e005a010-c116-42cf-ac78-01b8290d2aaa"),
                FirstName = "Colt",
                LastName = "Small",
                CreatedAt = DateTime.ParseExact("05-02-2024", "dd-MM-yyyy", CultureInfo.InvariantCulture),
                IsActive = true,
                Email = "colt@gmail.com"
            }
        };

        _userRepository
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), CancellationToken.None)
            .Returns(Task.FromResult(users));
        _mapper
            .Map<GetUserDto>(users[0])
            .Returns(getUserDtos[0]);
        _mapper
            .Map<GetUserDto>(users[1])
            .Returns(getUserDtos[1]);

        var query = new GetAllUsersQuery(1, 2);

        // Act
        var result = await _getAllUsersQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "Expected success flag to be true");
            Assert.That(result.IsFailed, Is.False, "Expected failure flag to be false");
            Assert.That(result.ValueOrDefault, Is.Not.Null, "Expected non-null Value");
            Assert.That(result.Value.Count, Is.EqualTo(2), "Value should have exactly 2 items");

            var dto1 = result.Value[0];
            Assert.That(dto1.Id, Is.EqualTo(guid1), "Id should match");
            Assert.That(dto1.Email, Is.EqualTo("bob@gmail.com"), "Email should match");
            Assert.That(dto1.FirstName, Is.EqualTo("Bob"), "FirstName should match");
            Assert.That(dto1.LastName, Is.EqualTo("Tall"), "LastName should match");
            Assert.That(dto1.CreatedAt, Is.EqualTo(users[0].CreatedAt).Within(TimeSpan.FromSeconds(1)), "CreatedAt should match");

            var dto2 = result.Value[1];
            Assert.That(dto2.Id, Is.EqualTo(guid2), "Id should match");
            Assert.That(dto2.Email, Is.EqualTo("colt@gmail.com"), "Email should match");
            Assert.That(dto2.FirstName, Is.EqualTo("Colt"), "FirstName should match");
            Assert.That(dto2.LastName, Is.EqualTo("Small"), "LastName should match");
            Assert.That(dto2.CreatedAt, Is.EqualTo(users[1].CreatedAt).Within(TimeSpan.FromSeconds(1)), "CreatedAt should match");
        });

        await _userRepository.Received(1).GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), CancellationToken.None);
        _mapper.Received(2).Map<GetUserDto>(Arg.Any<User>());
    }

    [Test]
    public async Task Handle_UsersNotFound_ReturnsFailure_DoesNotCallMapper()
    {
        // Arrange
        _userRepository
            .GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), CancellationToken.None)
            .Returns(new List<User>());

        var query = new GetAllUsersQuery(1, 3);

        // Act
        var result = await _getAllUsersQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False, "Expected success flag to be false");
            Assert.That(result.IsFailed, Is.True, "Expected failure flag to be true");
            Assert.That(result.ValueOrDefault, Is.Null, "Expected null Value");
            Assert.That(result.Errors.Count, Is.EqualTo(1), "Should have exactly one error");
            Assert.That(result.Errors[0].Message, Is.EqualTo("No users found."), "Expected error message: 'User not found'.");
        });

        await _userRepository.Received(1).GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), CancellationToken.None);

        _mapper.DidNotReceiveWithAnyArgs().Map<GetUserDto>(null);
    }
}
