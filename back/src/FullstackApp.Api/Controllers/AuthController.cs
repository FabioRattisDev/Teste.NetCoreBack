using Microsoft.AspNetCore.Mvc;
using MediatR;
using FullstackApp.Application.Users.Commands.RegisterUser;
using FullstackApp.Application.Users.Commands.LoginUser;

namespace FullstackApp.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand cmd)
        {
            var id = await _mediator.Send(cmd);
            return Ok(new { userId = id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserCommand cmd)
        {
            var token = await _mediator.Send(cmd);
            return Ok(new { token });
        }
    }
}
