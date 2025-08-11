using AutoMapper;
using NSubstitute;
using UserService.Application.CommandsQueries.Queries;
using UserService.Application.Interfaces;

namespace UserService.UnitTests.Application.QueryTests;

[TestFixture]
public class GetUserByIdQueryHandlerTests
{
    private IUserRepository _userRepository;
    private IMapper _mapper;
    private GetUserByIdQueryHandler _getUserByIdQueryHandler;

    [SetUp]
    public void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();

        _getUserByIdQueryHandler = new GetUserByIdQueryHandler(_userRepository, _mapper);
    }

    [Test]
    public async Task Handle_GetUserByIdQuery_ReturnsUserDto()
    {

    }
}