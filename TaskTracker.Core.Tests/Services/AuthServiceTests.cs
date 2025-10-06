using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TaskTracker.Core.Interfaces;
using TaskTracker.Infrastructure.Services;
using Xunit;

namespace TaskTracker.Core.Tests.Services;

public class AuthServiceTests
{
    private readonly IAuthService _authService;
    private readonly Mock<IConfiguration> _mockConfig;

    public AuthServiceTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockConfig.Setup(x => x["Jwt:Key"]).Returns("TestKeyThatIsLongEnoughForHmacSha256!");
        _mockConfig.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfig.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");

        _authService = new AuthService(_mockConfig.Object, Mock.Of<ILogger<AuthService>>());
    }

    [Fact]
    public void HashPassword_ShouldReturnConsistentHash()
    {
        // Arrange
        var password = "testpassword";

        // Act
        var hash1 = _authService.HashPassword(password);
        var hash2 = _authService.HashPassword(password);

        // Assert
        Assert.Equal(hash1, hash2);
        Assert.NotNull(hash1);
        Assert.NotEmpty(hash1);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "testpassword";
        var hash = _authService.HashPassword(password);

        // Act
        var result = _authService.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    {
        // Arrange
        var correctPassword = "testpassword";
        var wrongPassword = "wrongpassword";
        var hash = _authService.HashPassword(correctPassword);

        // Act
        var result = _authService.VerifyPassword(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }
}