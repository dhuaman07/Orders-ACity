using MediatR;
using SOL.MS.Security.Application.Common;
using SOL.MS.Security.Application.DTOs.Auth;
using SOL.MS.Security.Application.DTOs.Users;
using SOL.MS.Security.Domain.Repositories;
using SOL.MS.Security.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;
        private const int RefreshTokenExpirationDays = 1;
        private const int RefreshTokenExpirationMinutes = 3;
        private const int AccessTokenExpirationMinutes = 1;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenGenerator tokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Result<LoginResponseDto>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {        
            var user = await _unitOfWork.Users.GetByEmailAsync(
                request.Email,
                cancellationToken);

            if (user == null)
                return Result.Failure<LoginResponseDto>("Invalid email or password");
          
            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return Result.Failure<LoginResponseDto>("Invalid email or password");
      
            if (!user.IsActive)
                return Result.Failure<LoginResponseDto>("User account is inactive");

            var accessToken = _tokenGenerator.GenerateAccessToken(
                user.Id.ToString(),
                user.Email,
                user.Username);

            var refreshToken = _tokenGenerator.GenerateRefreshToken();
     
            var refreshTokenEntity = user.AddRefreshToken(
                refreshToken,
                //DateTime.UtcNow.AddMinutes(RefreshTokenExpirationMinutes),
                DateTime.UtcNow.AddDays(RefreshTokenExpirationDays),
                request.IpAddress);
       
            user.UpdateLastLogin();
         
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = AccessTokenExpirationMinutes * 60, // seconds
                User = new UserDto
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            };

            return Result.Success(response, "Login successful");
        }
    }
}
