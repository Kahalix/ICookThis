using ICookThis.Modules.Users.Dtos;
using ICookThis.Shared.Dtos;
using ICookThis.Modules.Users.Entities;

namespace ICookThis.Modules.Users.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserResponse>> GetPagedAsync(
            int page,
            int pageSize,
            string? search,
            UserSortBy sortBy,
            SortOrder sortOrder,
            UserStatus? statusFilter,
            UserRole? roleFilter);
        Task<UserResponse?> GetByIdAsync(int id);
        Task<PublicUserResponse?> GetPublicByIdAsync(int id);
        Task<UserResponse> CreateAsync(NewUserRequest dto, int currentUserId);
        Task<UserResponse> UpdateAsync(
            int id,
            UpdateUserRequest dto,
            int currentUserId);
        Task DeleteProfileImageAsync(int id, int currentUserId);
        Task DeleteBannerImageAsync(int id, int currentUserId);
        Task DeleteAsync(int id, int currentUserId);
        Task<UserResponse> ChangeStatusAsync(
            int id,
            ChangeUserStatusRequest dto,
            int currentUserId);
        Task<UserResponse> ChangeRoleAsync(
            int id,
            ChangeUseRoleRequest dto,
            int currentUserId);
    }
}
