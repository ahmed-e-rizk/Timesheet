using Xunit;
using Moq;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System;
using DTO.Auth;
using Timesheet.Core.Entites;
using Timesheet.DTO.Auth;
using BLL.Auth;
using Helper;
using Repositroy;
using Timesheet.Repositroy.Infrastructure;
using BLL.BaseResponse;
using DTO.Eunms;
using System.Collections.Generic;
using Timesheet.Helper;
using System.Linq.Expressions;

public class AuthBLLTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRepository<User>> _userRepoMock = new();
    private readonly Mock<IRepository<RefreshToken>> _refreshTokenRepoMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly AuthSetting _authSettings = new()
    {
        Jwt = new JwtSetting
        {
            Secret = "ThisIsASecretKeyForJwtTokenValidation",
            Issuer = "TestIssuer",
            TokenExpiryTime = TimeSpan.FromMinutes(30),
            RefreshToken = new RefreshTokenSetting
            {
                TokenLength = 10,
                RefreshTokenExpiryInMonths = 1
            }
        }
    };

    private AuthBLL CreateSut() =>
        new AuthBLL(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            Options.Create(_authSettings),
            _passwordHasherMock.Object,
            _userRepoMock.Object,
            _refreshTokenRepoMock.Object
        );

    [Fact]
    public async Task RegisterAsync_Should_Return_Success_When_User_Registered()
    {
        // Arrange
        var sut = CreateSut();
        var registerDto = new RegisterDto { Email = "test@example.com", Password = "password123", Name = "Test User" };
        var mappedUser = new User { Email = registerDto.Email, Password = "hashed", Name = registerDto.Name };
        _userRepoMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);

        _passwordHasherMock.Setup(x => x.HashPassword(registerDto.Password)).Returns("hashed");
        _mapperMock.Setup(x => x.Map<User>(registerDto)).Returns(mappedUser);
        var user = new User { Email = "test@example.com", Password = "hashedpassword", Name = "Test User" };

        _userRepoMock
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .ReturnsAsync(1); 
        // Act
        var result = await sut.RegisterAsync(registerDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Token_When_Credentials_Valid()
    {
        // Arrange
        var sut = CreateSut();
        var loginDto = new LoginDto { Email = "user@example.com", Password = "password" };
        var user = new User { Id = 1, Email = loginDto.Email, Password = "hashed", Name = "Test" };

        _userRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(loginDto.Password, user.Password)).Returns(true);

        // Act
        var result = await sut.LoginAsync(loginDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Data.Token));
        Assert.False(string.IsNullOrWhiteSpace(result.Data.RefreshToken));
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Error_When_Password_Is_Invalid()
    {
        var sut = CreateSut();
        var loginDto = new LoginDto { Email = "wrong@example.com", Password = "wrongpass" };
        var user = new User { Id = 1, Email = loginDto.Email, Password = "hashed" };
        _userRepoMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(loginDto.Password, user.Password)).Returns(false);

        var result = await sut.LoginAsync(loginDto);

        Assert.False(result.IsSuccess);
        Assert.Equal((int)MessageCodes.InvalidLoginCredentials, 1005);
    }
}
