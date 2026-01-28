using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Domain.Entities
{

    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

     
        public ICollection<RefreshToken> RefreshTokens { get; private set; }

       
        private User()
        {
            RefreshTokens = new List<RefreshToken>();
        }

       
        public static User Create(
            string email,
            string username,
            string passwordHash,
            string firstName,
            string lastName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required", nameof(username));

            return new User
            {
                Id = Guid.NewGuid(),
                Email = email.ToLowerInvariant(),
                Username = username.ToLowerInvariant(),
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                RefreshTokens = new List<RefreshToken>()
            };
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public RefreshToken AddRefreshToken(string token, DateTime expiresAt, string ipAddress)
        {
            var refreshToken = RefreshToken.Create(Id, token, expiresAt, ipAddress);
            RefreshTokens.Add(refreshToken);
            return refreshToken;
        }
    }
}
