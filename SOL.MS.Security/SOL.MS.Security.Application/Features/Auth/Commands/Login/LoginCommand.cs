using MediatR;
using SOL.MS.Security.Application.Common;
using SOL.MS.Security.Application.DTOs.Auth;

namespace SOL.MS.Security.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<Result<LoginResponseDto>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
    }
}
