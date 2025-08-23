using AutoMapper;
using FluentValidation;
using NSubstitute;
using MFO.UserService.Application.CommandsQueries.Commands;
using MFO.Contracts.User.DTOs;
using MFO.UserService.Application.Interfaces;

namespace MFO.UserService.UnitTests.Application.CommandTests;

[TestFixture]
public class CreateUserCommandHandlerTests
{
    private IUserRepository _userRepository;
    private IMapper _mapper;
    private IValidator<CreateUserDto> _validator;
    private IPasswordHasherService _passwordHasherService;
    private CreateUserCommandHandler _createUserCommandHandler;

    [SetUp]
    public virtual void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _validator = Substitute.For<IValidator<CreateUserDto>>();
        _passwordHasherService = Substitute.For<IPasswordHasherService>();
        _createUserCommandHandler = new CreateUserCommandHandler(_userRepository, _mapper, _validator, _passwordHasherService);
    }

    [Test]
    public void Test1()
    {

        Assert.Pass();
    }
}