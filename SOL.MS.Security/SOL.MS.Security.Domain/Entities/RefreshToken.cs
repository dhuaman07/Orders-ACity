using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string CreatedByIp { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string? RevokedByIp { get; private set; }
        public string? ReplacedByToken { get; private set; }

 
        public User User { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, string token, DateTime expiresAt, string ipAddress)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        public void Revoke(string ipAddress, string replacedByToken = null)
        {
            RevokedAt = DateTime.UtcNow;
            RevokedByIp = ipAddress;
            ReplacedByToken = replacedByToken;
        }
    }
}
