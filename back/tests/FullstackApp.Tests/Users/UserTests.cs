using Moq;
using FullstackApp.Domain.Entities;
using FullstackApp.Infrastructure.Repositories.Interface;
using FullstackApp.Domain.Services.Interface;
using FullstackApp.Application.Users.Commands.RegisterUser;
using FullstackApp.Application.Users.Commands.LoginUser;

namespace FullstackApp.Tests.Users
{
    public class UserTests
    {
        [Fact]
        public async Task RegisterUser_ShouldReturnGuid()
        {
            // Arrange
            var userRepo = new Mock<IUserRepository>();
            var repo = new Mock<IRepository<User>>();

            userRepo.Setup(r => r.EmailExistsAsync("fabio@test.com"))
                    .ReturnsAsync(false);

            repo.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            repo.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var handler = new RegisterUserHandler(userRepo.Object, repo.Object);
            var cmd = new RegisterUserCommand("Fabio", "fabio@test.com", "123456");

            var id = await handler.Handle(cmd, CancellationToken.None);


            Assert.NotEqual(Guid.Empty, id);

            userRepo.Verify(r => r.EmailExistsAsync("fabio@test.com"), Times.Once);
            repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            repo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
