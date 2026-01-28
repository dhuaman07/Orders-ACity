using MediatR;
using SOL.MS.Security.Application.Common;
using SOL.MS.Security.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<Result<RegisterResponseDto>>
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
