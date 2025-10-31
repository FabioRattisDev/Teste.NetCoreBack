using FullstackApp.Application.Users.Dto;
using FullstackApp.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullstackApp.Application.Users.Queries.GetUsers
{
    public record GetUsersQuery() : IRequest<List<UserDto>>;
}
