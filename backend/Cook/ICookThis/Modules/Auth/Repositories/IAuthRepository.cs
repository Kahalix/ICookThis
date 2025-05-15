using ICookThis.Data;
using ICookThis.Modules.Auth.Entities;

namespace ICookThis.Modules.Auth.Repositories
{
    public interface IAuthRepository
    {
        Task AddTokenAsync(UserToken token);
        Task<UserToken?> GetTokenAsync(Guid tokenId, TokenType type);
        Task DeleteTokenAsync(UserToken token);
    }
}