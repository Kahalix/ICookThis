using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ICookThis.Modules.Auth.Dtos;
using ICookThis.Modules.Auth.Entities;
using ICookThis.Modules.Auth.Repositories;
using ICookThis.Modules.jwt.Services;
using ICookThis.Modules.Users.Entities;
using ICookThis.Modules.Users.Repositories;
using ICookThis.Shared.Dtos;
using ICookThis.Utils.Email;
using Microsoft.Extensions.Configuration;

namespace ICookThis.Modules.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IAuthRepository _authRepo;
        private readonly IJwtService _jwt;
        private readonly IMailService _mail;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IEmailBuilder _emailBuilder;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthService(
            IUserRepository userRepo,
            IAuthRepository authRepo,
            IJwtService jwt,
            IMailService mail,
            IPasswordHasher<User> hasher,
            IEmailBuilder emailBuilder,
            IMapper mapper,
            IConfiguration config)
        {
            _userRepo = userRepo;
            _authRepo = authRepo;
            _jwt = jwt;
            _mail = mail;
            _hasher = hasher;
            _emailBuilder = emailBuilder;
            _mapper = mapper;
            _config = config;
        }

        public async Task RegisterAsync(RegisterRequest dto)
        {
            var user = _mapper.Map<User>(dto);
            user.Password = _hasher.HashPassword(user, dto.Password);
            user.Status = UserStatus.Pending;
            user.Role = UserRole.User;
            user.CreatedAt = DateTime.UtcNow;

            await _userRepo.AddAsync(user);

            var token = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = TokenType.EmailConfirmation,
                Expiry = DateTime.UtcNow.AddDays(14)
            };
            await _authRepo.AddTokenAsync(token);

            var confirmUrl = $"{_config["App:FrontendUrl"]}/confirm-email?token={token.Id}";
            var (subject, body) = _emailBuilder.BuildConfirmationEmail(confirmUrl);
            await _mail.SendAsync(user.Email, subject, body);
        }

        public async Task<bool> ConfirmEmailAsync(Guid tokenId)
        {
            var token = await _authRepo.GetTokenAsync(tokenId, TokenType.EmailConfirmation);
            if (token == null || token.Expiry < DateTime.UtcNow) return false;

            var user = await _userRepo.GetByIdAsync(token.UserId)
                       ?? throw new KeyNotFoundException("User not found");
            user.Status = UserStatus.Approved;
            await _userRepo.UpdateAsync(user);
            await _authRepo.DeleteTokenAsync(token);
            return true;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest dto)
        {
            var user = await _userRepo.FindByUsernameOrEmailAsync(dto.UserNameOrEmail);
            if (user == null ||
                _hasher.VerifyHashedPassword(user, user.Password, dto.Password)
                    != PasswordVerificationResult.Success)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (user.Status == UserStatus.Pending)
                throw new UnauthorizedAccessException("Account not confirmed");

            if (user.Status == UserStatus.Deleted)
                throw new UnauthorizedAccessException("Account is unavailable");

            if (user.Status == UserStatus.Suspended)
                throw new UnauthorizedAccessException("Account is suspended");

            var tokenStr = _jwt.GenerateToken(user.Id, user.UserName, user.Role.ToString());
            var response = _mapper.Map<AuthResponse>(user);
            response.Token = tokenStr;
            response.Expires = DateTime.UtcNow.AddMinutes(
                                 int.Parse(_config["Jwt:ExpireMinutes"]!));
            return response;
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest dto)
        {
            var user = await _userRepo.FindByEmailAsync(dto.Email);
            if (user == null || user.Status != UserStatus.Approved) return;

            var token = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = TokenType.PasswordReset,
                Expiry = DateTime.UtcNow.AddHours(1)
            };
            await _authRepo.AddTokenAsync(token);

            var resetUrl = $"{_config["App:FrontendUrl"]}/reset-password?token={token.Id}";
            var (subject, body) = _emailBuilder.BuildPasswordResetEmail(resetUrl);
            await _mail.SendAsync(user.Email, subject, body);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest dto)
        {
            var token = await _authRepo.GetTokenAsync(dto.Token, TokenType.PasswordReset);
            if (token == null || token.Expiry < DateTime.UtcNow) return false;

            var user = await _userRepo.GetByIdAsync(token.UserId)
                       ?? throw new KeyNotFoundException("User not found");
            user.Password = _hasher.HashPassword(user, dto.NewPassword);
            await _userRepo.UpdateAsync(user);
            await _authRepo.DeleteTokenAsync(token);
            return true;
        }

        public async Task CleanupPendingAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-14);
            var pending = await _userRepo.GetPendingOlderThanAsync(cutoff);
            foreach (var u in pending)
                await _userRepo.DeleteAsync(u.Id);
        }
    }
}
