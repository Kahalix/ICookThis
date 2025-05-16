using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using ICookThis.Modules.Users.Dtos;
using ICookThis.Modules.Users.Entities;
using ICookThis.Modules.Users.Repositories;
using ICookThis.Shared.Dtos;
using ICookThis.Utils.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IWebHostEnvironment _env;
        private readonly IMailService _mail;
        private readonly IEmailBuilder _emailBuilder;
        private readonly IConfiguration _config;

        public UserService(
            IUserRepository repo,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            IWebHostEnvironment env,
            IMailService mail,
            IEmailBuilder emailBuilder,
            IConfiguration config)
        {
            _repo = repo;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _env = env;
            _mail = mail;
            _emailBuilder = emailBuilder;
            _config = config;
        }

        public async Task<PagedResult<UserResponse>> GetPagedAsync(
            int page,
            int pageSize,
            string? search,
            UserSortBy sortBy,
            SortOrder sortOrder,
            UserStatus? statusFilter,
            UserRole? roleFilter)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var (users, total) = await _repo.GetPagedAsync(
                page, pageSize,
                search, sortBy, sortOrder,
                statusFilter, roleFilter);

            var dtos = _mapper.Map<List<UserResponse>>(users);
            return new PagedResult<UserResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                Items = dtos
            };
        }

        public async Task<PublicUserResponse?> GetPublicByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"User with id {id} not found");

            if (user.Status is not UserStatus.Approved)
                throw new UnauthorizedAccessException("You can't view this users");

            return _mapper.Map<PublicUserResponse>(user);
        }

        public async Task<UserResponse?> GetByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id)
                          ?? throw new KeyNotFoundException($"User with id {id} not found");

            return _mapper.Map<UserResponse>(user);
        }

        public async Task<UserResponse> CreateAsync(NewUserRequest dto, int currentUserId)
        {
            var current = await _repo.GetByIdAsync(currentUserId)
                          ?? throw new KeyNotFoundException($"User {currentUserId} not found");

            if (current.Role != UserRole.Admin)
                throw new UnauthorizedAccessException("Only Admin can create users");

            var entity = _mapper.Map<User>(dto);
            entity.Password = _passwordHasher.HashPassword(entity, dto.Password);
            entity.Status = UserStatus.Pending;
            entity.Role = UserRole.User;
            entity.CreatedAt = DateTime.UtcNow;

            var created = await _repo.AddAsync(entity);
            var userDto = _mapper.Map<UserResponse>(created);

            var (subject, body) = _emailBuilder.BuildAdminCreatedUserEmail(created.UserName);
            await _mail.SendAsync(created.Email, subject, body);
            
            return userDto;
        }

        public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest dto, int currentUserId)
        {
            var user = await _repo.GetByIdAsync(id)
                     ?? throw new KeyNotFoundException($"User {id} not found");

            var current = await _repo.GetByIdAsync(currentUserId)
                         ?? throw new KeyNotFoundException($"User {currentUserId} not found");

            EnsureApproved(current);

            bool isSelf = id == currentUserId;
            bool isAdmin = current.Role == UserRole.Admin;
            if (!isSelf && !isAdmin)
                throw new UnauthorizedAccessException("You do not have permission to update this user");

            if (dto.ProfileImageFile != null)
            {
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    var old = Path.Combine(_env.WebRootPath,
                               user.ProfileImage.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (File.Exists(old)) File.Delete(old);
                }
                var upProf = Path.Combine(_env.WebRootPath, "images", "users", "profiles");
                Directory.CreateDirectory(upProf);
                var fn = $"{Guid.NewGuid()}{Path.GetExtension(dto.ProfileImageFile.FileName)}";
                var fp = Path.Combine(upProf, fn);
                using var fs = File.Create(fp);
                await dto.ProfileImageFile.CopyToAsync(fs);
                user.ProfileImage = Path.Combine("images", "users", "profiles", fn).Replace("\\", "/");
            }

            if (dto.BannerImageFile != null)
            {
                if (!string.IsNullOrEmpty(user.BannerImage))
                {
                    var old = Path.Combine(_env.WebRootPath,
                               user.BannerImage.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (File.Exists(old)) File.Delete(old);
                }
                var upBan = Path.Combine(_env.WebRootPath, "images", "users", "banners");
                Directory.CreateDirectory(upBan);
                var fn = $"{Guid.NewGuid()}{Path.GetExtension(dto.BannerImageFile.FileName)}";
                var fp = Path.Combine(upBan, fn);
                using var fs = File.Create(fp);
                await dto.BannerImageFile.CopyToAsync(fs);
                user.BannerImage = Path.Combine("images", "users", "banners", fn).Replace("\\", "/");
            }

            if (!string.IsNullOrEmpty(dto.Password))
                user.Password = _passwordHasher.HashPassword(user, dto.Password);

            _mapper.Map(dto, user);

            var updated = await _repo.UpdateAsync(user);
            return _mapper.Map<UserResponse>(updated);
        }

        public async Task DeleteProfileImageAsync(int id, int currentUserId)
        {
            var user = await _repo.GetByIdAsync(id)
                     ?? throw new KeyNotFoundException($"User {id} not found");
            if (id != currentUserId &&
                !((await _repo.GetByIdAsync(currentUserId))?.Role
                   is UserRole.Admin or UserRole.Moderator))
                throw new UnauthorizedAccessException("You do not have permission to update this user");

            if (!string.IsNullOrEmpty(user.ProfileImage))
            {
                var path = Path.Combine(_env.WebRootPath,
                           user.ProfileImage.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(path)) File.Delete(path);
                user.ProfileImage = null;
                await _repo.UpdateAsync(user);
            }
        }

        public async Task DeleteBannerImageAsync(int id, int currentUserId)
        {
            var user = await _repo.GetByIdAsync(id)
                     ?? throw new KeyNotFoundException($"User {id} not found");
            if (id != currentUserId &&
                !((await _repo.GetByIdAsync(currentUserId))?.Role
                   is UserRole.Admin or UserRole.Moderator))
                throw new UnauthorizedAccessException("You do not have permission to update this user");

            if (!string.IsNullOrEmpty(user.BannerImage))
            {
                var path = Path.Combine(_env.WebRootPath,
                           user.BannerImage.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(path)) File.Delete(path);
                user.BannerImage = null;
                await _repo.UpdateAsync(user);
            }
        }

        public async Task<UserResponse> ChangeStatusAsync(int id, ChangeUserStatusRequest dto, int currentUserId)
        {
            var current = await _repo.GetByIdAsync(currentUserId)
                          ?? throw new KeyNotFoundException($"User {currentUserId} not found");

            EnsureApproved(current);

            var user = await _repo.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"User {id} not found");

            bool isAdminOrMod = current.Role == UserRole.Admin || current.Role == UserRole.Moderator;
            bool isOwner = id == currentUserId;

            if (isAdminOrMod)
            {
                if (user.Role == UserRole.Admin)
                    throw new UnauthorizedAccessException("You cannot change the status of an Admin user");

                await _repo.SetStatusAsync(id, dto.Status);
            }
            else if (isOwner && dto.Status == UserStatus.Deleted)
            {
                await _repo.SetStatusAsync(id, UserStatus.Deleted);
            }
            else
            {
                throw new UnauthorizedAccessException("You do not have permission to change this user’s status");
            }


            var updated = await _repo.GetByIdAsync(id)
                          ?? throw new KeyNotFoundException($"User {id} not found");
            var resultDto = _mapper.Map<UserResponse>(updated);

            var (subject, body) = _emailBuilder.BuildStatusChangedEmail(updated.UserName, updated.Status);
            await _mail.SendAsync(updated.Email, subject, body);

            return resultDto;
        }


        public async Task<UserResponse> ChangeRoleAsync(int id, ChangeUseRoleRequest dto, int currentUserId)
        {
            var current = await _repo.GetByIdAsync(currentUserId)
                        ?? throw new KeyNotFoundException($"User {currentUserId} not found");

            if (current.Role != UserRole.Admin)
                throw new UnauthorizedAccessException("Only Admin can change role");

            await _repo.SetRoleAsync(id, dto.Role);
            var updated = await _repo.GetByIdAsync(id)
                          ?? throw new KeyNotFoundException($"User {id} not found");
            var resultDto = _mapper.Map<UserResponse>(updated);

            var (subject, body) = _emailBuilder.BuildRoleChangedEmail(updated.UserName, updated.Role);
            await _mail.SendAsync(updated.Email, subject, body);

            return resultDto;
        }

        public async Task DeleteAsync(int id, int currentUserId)
        {
            var current = await _repo.GetByIdAsync(currentUserId)
                          ?? throw new KeyNotFoundException($"User {currentUserId} not found");

            if (current.Role != UserRole.Admin)
                throw new UnauthorizedAccessException("Only Admin can delete users");

            var user = await _repo.GetByIdAsync(id);
            if (user == null) return;

            if (!string.IsNullOrEmpty(user.ProfileImage))
            {
                var profilePath = Path.Combine(
                    _env.WebRootPath,
                    user.ProfileImage.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(profilePath))
                    File.Delete(profilePath);
            }

            if (!string.IsNullOrEmpty(user.BannerImage))
            {
                var bannerPath = Path.Combine(
                    _env.WebRootPath,
                    user.BannerImage.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(bannerPath))
                    File.Delete(bannerPath);
            }

            await _repo.DeleteAsync(id);
        }

        private void EnsureApproved(User user)
        {
            if (user.Status != UserStatus.Approved
                && user.Role is not UserRole.Admin)
            {
                throw new UnauthorizedAccessException("Only approved users can perform this operation.");
            }
        }


    }
}
