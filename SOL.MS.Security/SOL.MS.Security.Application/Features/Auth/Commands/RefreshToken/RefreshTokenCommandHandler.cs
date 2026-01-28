using MediatR;
using SOL.MS.Security.Application.Common;
using SOL.MS.Security.Application.DTOs.Auth;
using SOL.MS.Security.Domain.Repositories;
using SOL.MS.Security.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenGenerator _tokenGenerator;
        private const int RefreshTokenExpirationDays = 1;
        private const int RefreshTokenExpirationMinutes = 3;
        private const int AccessTokenExpirationMinutes = 1;

        public RefreshTokenCommandHandler(
            IUnitOfWork unitOfWork,
            ITokenGenerator tokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Result<RefreshTokenResponseDto>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {

            var refreshToken = await _unitOfWork.RefreshTokens
                .GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (refreshToken == null)
                return Result.Failure<RefreshTokenResponseDto>("Invalid refresh token");

      
            if (!refreshToken.IsActive)
                return Result.Failure<RefreshTokenResponseDto>(
                    refreshToken.IsExpired
                        ? "Refresh token has expired"
                        : "Refresh token has been revoked");

         
            var user = await _unitOfWork.Users
                .GetByIdAsync(refreshToken.UserId, cancellationToken);

            if (user == null || !user.IsActive)
                return Result.Failure<RefreshTokenResponseDto>("User not found or inactive");

      
            var newAccessToken = _tokenGenerator.GenerateAccessToken(
                user.Id.ToString(),
                user.Email,
                user.Username);

            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

      
            refreshToken.Revoke(request.IpAddress, newRefreshToken);

          
            var newRefreshTokenEntity = user.AddRefreshToken(
                newRefreshToken,
                //DateTime.UtcNow.AddMinutes(RefreshTokenExpirationMinutes),
                DateTime.UtcNow.AddDays(RefreshTokenExpirationDays),
                request.IpAddress);

      
            await _unitOfWork.SaveChangesAsync(cancellationToken);

    
            var response = new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenType = "Bearer",
                ExpiresIn = AccessTokenExpirationMinutes * 60
            };

            return Result.Success(response, "Token refreshed successfully");
        }
    }
}
