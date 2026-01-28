using Microsoft.EntityFrameworkCore;
using SOL.MS.Security.Domain.Entities;
using SOL.MS.Security.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Infraestructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetByTokenAsync(
            string token,
            CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task<RefreshToken> AddAsync(
            RefreshToken refreshToken,
            CancellationToken cancellationToken = default)
        {
            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            return refreshToken;
        }

        public Task UpdateAsync(
            RefreshToken refreshToken,
            CancellationToken cancellationToken = default)
        {
            _context.RefreshTokens.Update(refreshToken);
            return Task.CompletedTask;
        }

        public async Task RevokeAllUserTokensAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var token in tokens)
            {
                token.Revoke("System");
            }
        }
    }
}
