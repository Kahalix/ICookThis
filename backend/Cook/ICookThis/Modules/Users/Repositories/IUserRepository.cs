using ICookThis.Modules.Users.Entities;
using ICookThis.Shared.Dtos;
using ICookThis.Modules.Users.Dtos;

namespace ICookThis.Modules.Users.Repositories
{
    public interface IUserRepository
    {
        Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string? search,
            UserSortBy sortBy,
            SortOrder sortOrder,
            UserStatus? statusFilter,
            UserRole? roleFilter);

        Task<User?> GetByIdAsync(int id);
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByUsernameOrEmailAsync(string usernameOrEmail);
        Task<IEnumerable<User>> GetPendingOlderThanAsync(DateTime cutoff);
        Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<int> ids);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task SetStatusAsync(int id, UserStatus status);
        Task SetRoleAsync(int id, UserRole role);
        Task SetTrustFactorAsync(int userId, decimal trustFactor);
    }
}