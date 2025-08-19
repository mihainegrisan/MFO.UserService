using AutoMapper;
using FluentValidation;
using NSubstitute;
using MFO.UserService.Application.CommandsQueries.Commands;
using MFO.UserService.Application.DTOs;
using MFO.UserService.Application.Interfaces;

namespace MFO.UserService.UnitTests.Application.CommandTests;

[TestFixture]
public class UpdateUserCommandHandlerTests
{
    private IUserRepository _userRepository;
    private IMapper _mapper;
    private IValidator<UpdateUserDto> _validator;
    private IPasswordHasherService _passwordHasherService;
    private UpdateUserCommandHandler _updateUserCommandHandler;

    [SetUp]
    public virtual void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _validator = Substitute.For<IValidator<UpdateUserDto>>();
        _passwordHasherService = Substitute.For<IPasswordHasherService>();
        _updateUserCommandHandler = new UpdateUserCommandHandler(_userRepository, _mapper, _validator, _passwordHasherService);
    }

    [Test]
    public void Test1()
    {

        Assert.Pass();
    }
}