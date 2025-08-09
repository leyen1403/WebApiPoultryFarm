using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiPoultryFarm.Api.Models;
using WebApiPoultryFarm.Application.Users.LoginUser;
using WebApiPoultryFarm.Application.Users.Refresh;
using WebApiPoultryFarm.Application.Users.Register;

namespace WebApiPoultryFarm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
        {
            // Create a new user account
            var result = await _mediator.Send(command, ct);
            return Ok(ApiResponse<RegisterUserResult>.Ok(result, "Đăng ký tài khoản thành công!"));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(ApiResponse<LoginUserResult>.Ok(result, "Đăng nhập thành công!"));
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(ApiResponse<RefreshTokenResult>.Ok(result, "Làm mới token thành công!"));
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            // Claims issued by JwtTokenService (uid, uname, email)
            var uid = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            var uname = User.Claims.FirstOrDefault(c => c.Type == "uname")?.Value;
            var email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            var data = new { userId = uid, userName = uname, email };
            return Ok(ApiResponse<object>.Ok(data));
        }
    }

}
