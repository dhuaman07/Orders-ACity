using SOL.MS.Security.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.MS.Security.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteAsync(User user, CancellationToken cancellationToken = default);
    }
}
