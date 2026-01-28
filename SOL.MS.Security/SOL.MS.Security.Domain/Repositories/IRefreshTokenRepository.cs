using SOL.MS.Security.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<RefreshToken> AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
