using TaskTracker.Core.Entities;

namespace TaskTracker.Core.Interfaces;
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task<bool> UserExistsAsync(string username, string email);
}