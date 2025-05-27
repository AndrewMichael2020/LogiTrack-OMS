using Xunit;
using LogiTrack.Controllers;
using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LogiTrack.Tests
{
    public class AuthControllerTests
    {
        private AuthController GetController(
            Mock<UserManager<ApplicationUser>> userManagerMock = null,
            Mock<SignInManager<ApplicationUser>> signInManagerMock = null,
            IConfiguration config = null)
        {
            userManagerMock ??= MockUserManager();
            signInManagerMock ??= MockSignInManager(userManagerMock.Object);
            config ??= new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                // Use a key of at least 256 bits (32+ ASCII chars) for HS256
                { "Jwt:Key", "supersecretkey1234supersecretkey1234" },
                { "Jwt:Issuer", "logitrack" },
                { "Jwt:Audience", "logitrack" }
            }).Build();

            return new AuthController(userManagerMock.Object, signInManagerMock.Object, config);
        }

        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<SignInManager<ApplicationUser>> MockSignInManager(UserManager<ApplicationUser> userManager)
        {
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            return new Mock<SignInManager<ApplicationUser>>(userManager, contextAccessor.Object, claimsFactory.Object, null, null, null, null);
        }

        [Fact]
        public async Task Register_Success()
        {
            var userManagerMock = MockUserManager();
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            var controller = GetController(userManagerMock);

            var result = await controller.Register(new RegisterModel
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password1"
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("successful", ok.Value.ToString());
        }

        [Fact]
        public async Task Register_Fails_If_Username_Exists()
        {
            var userManagerMock = MockUserManager();
            userManagerMock.Setup(x => x.FindByNameAsync("testuser"))
                .ReturnsAsync(new ApplicationUser());
            var controller = GetController(userManagerMock);

            var result = await controller.Register(new RegisterModel
            {
                Username = "testuser",
                Email = "test2@example.com",
                Password = "Password1"
            });

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Username already exists", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Login_Success_ReturnsToken()
        {
            var user = new ApplicationUser { UserName = "testuser", Id = "1" };
            var userManagerMock = MockUserManager();
            userManagerMock.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);

            var signInManagerMock = MockSignInManager(userManagerMock.Object);
            signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "Password1", false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var controller = GetController(userManagerMock, signInManagerMock);

            var result = await controller.Login(new LoginModel
            {
                Username = "testuser",
                Password = "Password1"
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("token", ok.Value.ToString());
        }

        [Fact]
        public async Task Login_Fails_With_Invalid_Credentials()
        {
            var userManagerMock = MockUserManager();
            userManagerMock.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync((ApplicationUser)null);
            var controller = GetController(userManagerMock);

            var result = await controller.Login(new LoginModel
            {
                Username = "testuser",
                Password = "wrong"
            });

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Contains("Invalid username or password", unauthorized.Value.ToString());
        }
    }
}
