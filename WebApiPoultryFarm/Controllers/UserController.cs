using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApiPoultryFarm.Api.Models;
using WebApiPoultryFarm.Application.Users;
using WebApiPoultryFarm.Application.Users.CreateUser;
using WebApiPoultryFarm.Application.Users.LoginUser;
using WebApiPoultryFarm.Share.Exeptions;

namespace WebApiPoultryFarm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(ApiResponse<UserResponse>.Ok(result, "Đăng ký thành công!"));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse<UserResponse>.Fail(ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(ApiResponse<UserResponse>.Ok(result, "Đăng nhập thành công!"));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponse<UserResponse>.Fail(ex.Message));
            }
        }
    }

}
