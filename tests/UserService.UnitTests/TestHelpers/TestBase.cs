using AutoMapper;
using FluentValidation;
using NSubstitute;
using UserService.Application.CommandsQueries.Queries;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.UnitTests.TestHelpers;

public class TestBase
{
    protected IUserRepository UserRepository;
    protected IMapper Mapper;
    protected IValidator<GetUserByEmailDto> Validator;

    [SetUp]
    public virtual void Setup()
    {
        UserRepository = Substitute.For<IUserRepository>();
        Mapper = Substitute.For<IMapper>();
        Validator = Substitute.For<IValidator<GetUserByEmailDto>>();
    }
}