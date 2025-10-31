using FullstackApp.Domain.Services.Interface;
using FullstackApp.Infrastructure.Repositories.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullstackApp.Application.Users.Commands.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, string>
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtService _jwt;

        public LoginUserHandler(IUserRepository userRepo, IJwtService jwt)
        {
            _userRepo = userRepo;
            _jwt = jwt;
        }

        public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Credenciais inválidas");

            return _jwt.GenerateToken(user.Id, user.Email);
        }
    }
}
