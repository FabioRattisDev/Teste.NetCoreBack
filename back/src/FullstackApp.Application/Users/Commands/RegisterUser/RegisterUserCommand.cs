using MediatR;

namespace FullstackApp.Application.Users.Commands.RegisterUser
{
    public record RegisterUserCommand(string Name, string Email, string Password) : IRequest<Guid>;
}
