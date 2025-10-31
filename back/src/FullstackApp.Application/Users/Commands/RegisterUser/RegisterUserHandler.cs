using FullstackApp.Domain.Entities;
using FullstackApp.Infrastructure.Repositories.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullstackApp.Application.Users.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepo;
        private readonly IRepository<User> _repo;

        public RegisterUserHandler(IUserRepository userRepo, IRepository<User> repo)
        {
            _userRepo = userRepo;
            _repo = repo;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepo.EmailExistsAsync(request.Email))
                throw new Exception("Email already exists");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };
            user.Created();

            await _repo.AddAsync(user);
            await _repo.SaveChangesAsync();

            return user.Id;
        }
    }
}
