using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SOL.MS.Security.Application.Common;
using SOL.MS.Security.Application.Features.Auth.Commands.Login;
using SOL.MS.Security.Application.Features.Auth.Commands.RefreshToken;
using SOL.MS.Security.Application.Features.Auth.Commands.Register;

namespace SOL.MS.Security.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
     
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);
          
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            command.IpAddress = GetIpAddress();
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);
           
            SetRefreshTokenCookie(result.Data.RefreshToken);
            
            var response = Result.Success(new
            {
                AccessToken = result.Data.AccessToken,
                TokenType = result.Data.TokenType,
                ExpiresIn = result.Data.ExpiresIn,
                User = result.Data.User
            }, result.Message);

            return Ok(response);
        }
       
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(Result.Failure<object>("Refresh token is required"));

            var command = new RefreshTokenCommand
            {
                RefreshToken = refreshToken,
                IpAddress = GetIpAddress()
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);
            
            SetRefreshTokenCookie(result.Data.RefreshToken);
            
            var response = Result.Success(new
            {
                AccessToken = result.Data.AccessToken,
                TokenType = result.Data.TokenType,
                ExpiresIn = result.Data.ExpiresIn
            }, result.Message);

            return Ok(response);
        }
       
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            return Ok(Result.Success("Logged out successfully"));
        }
       
        [Authorize]
        [HttpGet("user-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst("userId")?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var username = User.FindFirst("username")?.Value;

            var userData = new
            {
                UserId = userId,
                Email = email,
                Username = username,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            };

            return Ok(Result.Success(userData, "User retrieved successfully"));
        }

        #region Private Methods

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = System.DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        #endregion
    }
}
