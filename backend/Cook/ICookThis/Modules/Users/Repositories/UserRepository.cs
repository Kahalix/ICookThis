using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICookThis.Data;
using ICookThis.Modules.Users.Entities;
using ICookThis.Modules.Users.Dtos;
using ICookThis.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Modules.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CookThisDbContext _db;
        public UserRepository(CookThisDbContext db) => _db = db;

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            string? search,
            UserSortBy sortBy,
            SortOrder sortOrder,
            UserStatus? statusFilter,
            UserRole? roleFilter)
        {
            var query = _db.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(u =>
                    EF.Functions.Like(u.UserName, $"%{term}%") ||
                    EF.Functions.Like(u.Email, $"%{term}%"));
            }

            if (statusFilter.HasValue)
                query = query.Where(u => u.Status == statusFilter.Value);

            if (roleFilter.HasValue)
                query = query.Where(u => u.Role == roleFilter.Value);

            bool desc = sortOrder == SortOrder.Desc;
            query = sortBy switch
            {
                UserSortBy.CreatedAt =>
                    desc ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                UserSortBy.TrustFactor =>
                    desc ? query.OrderByDescending(u => u.TrustFactor) : query.OrderBy(u => u.TrustFactor),
                _ =>
                    desc ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName),
            };

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, total);
        }

        public Task<User?> GetByIdAsync(int id) =>
            _db.Users.FindAsync(id).AsTask();

        public async Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return Array.Empty<User>();

            return await _db.Users
                .Where(u => ids.Contains(u.Id))
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<User?> FindByEmailAsync(string email) =>
            _db.Users.SingleOrDefaultAsync(u => u.Email == email);

        public Task<User?> FindByUsernameOrEmailAsync(string usernameOrEmail) =>
            _db.Users.SingleOrDefaultAsync(u =>
                u.UserName == usernameOrEmail ||
                u.Email == usernameOrEmail);

        public Task<IEnumerable<User>> GetPendingOlderThanAsync(DateTime cutoff) =>
            _db.Users
               .Where(u => u.Status == UserStatus.Pending && u.CreatedAt < cutoff)
               .ToListAsync()
               .ContinueWith(t => (IEnumerable<User>)t.Result);

        public async Task<User> AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return;
            _db.Users.Remove(u);
            await _db.SaveChangesAsync();
        }

        public async Task SetStatusAsync(int id, UserStatus status)
        {
            var u = new User { Id = id };
            _db.Users.Attach(u);
            u.Status = status;
            await _db.SaveChangesAsync();
        }

        public async Task SetRoleAsync(int id, UserRole role)
        {
            var u = new User { Id = id };
            _db.Users.Attach(u);
            u.Role = role;
            await _db.SaveChangesAsync();
        }

        public async Task SetTrustFactorAsync(int userId, decimal trustFactor)
        {
            var u = new User { Id = userId };
            _db.Users.Attach(u);
            u.TrustFactor = trustFactor;
            await _db.SaveChangesAsync();
        }

    }
}
