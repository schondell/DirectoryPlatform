using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public async Task<User?> GetByUsernameAsync(string username)
        => await _dbSet.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        => await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

    public async Task<User?> GetByEmailVerificationTokenAsync(string token)
        => await _dbSet.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);

    public async Task<User?> GetByPasswordResetTokenAsync(string token)
        => await _dbSet.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
}
