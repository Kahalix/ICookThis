using ICookThis.Data;
using ICookThis.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Modules.Auth.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly CookThisDbContext _db;
        public AuthRepository(CookThisDbContext db) => _db = db;

        public Task AddTokenAsync(UserToken token)
        {
            _db.UserTokens.Add(token);
            return _db.SaveChangesAsync();
        }

        public Task<UserToken?> GetTokenAsync(Guid tokenId, TokenType type) =>
            _db.UserTokens
               .Where(t => t.Id == tokenId && t.Type == type)
               .FirstOrDefaultAsync();

        public Task DeleteTokenAsync(UserToken token)
        {
            _db.UserTokens.Remove(token);
            return _db.SaveChangesAsync();
        }
    }
}
