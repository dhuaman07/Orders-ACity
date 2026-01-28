using MediatR;
using SOL.MS.Security.Application.Common;
using SOL.MS.Security.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Result<RefreshTokenResponseDto>>
    {
        public string RefreshToken { get; set; }
        public string IpAddress { get; set; }
    }
}
