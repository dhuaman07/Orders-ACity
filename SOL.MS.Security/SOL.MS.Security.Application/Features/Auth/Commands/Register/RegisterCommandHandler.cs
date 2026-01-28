using MediatR;
using SOL.MS.Security.Application.Common;
using SOL.MS.Security.Application.DTOs.Auth;
using SOL.MS.Security.Domain.Entities;
using SOL.MS.Security.Domain.Repositories;
using SOL.MS.Security.Domain.Services;

namespace SOL.MS.Security.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler
         : IRequestHandler<RegisterCommand, Result<RegisterResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterCommandHandler(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<RegisterResponseDto>> Handle(RegisterCommand request,CancellationToken cancellationToken)
        {
          
            if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email, cancellationToken))
                return Result.Failure<RegisterResponseDto>("Email is already registered");

         
            if (await _unitOfWork.Users.ExistsByUsernameAsync(request.Username, cancellationToken))
                return Result.Failure<RegisterResponseDto>("Username is already taken");

         
            var passwordHash = _passwordHasher.HashPassword(request.Password);

      
            var user = User.Create(
                request.Email,
                request.Username,
                passwordHash,
                request.FirstName,
                request.LastName);

     
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

         
            var response = new RegisterResponseDto
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return Result.Success(response, "User registered successfully");
        }
    }
}
