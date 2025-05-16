using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Repositories;
using ICookThis.Modules.Reviews.Dtos;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Modules.Reviews.Repositories;
using ICookThis.Modules.Users.Entities;
using ICookThis.Modules.Users.Repositories;
using ICookThis.Shared.Dtos;
using ICookThis.Utils.Email;


namespace ICookThis.Modules.Reviews.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        private readonly IRecipeRepository _recipeRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IMailService _mail;
        private readonly IEmailBuilder _emails;
        private readonly IConfiguration _config;

        public ReviewService(
            IReviewRepository repo,
            IRecipeRepository recipeRepo,
            IUserRepository userRepo,
            IMapper mapper,
            IMailService mail,
            IEmailBuilder emails,
            IConfiguration config)
        {
            _repo = repo;
            _recipeRepo = recipeRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _mail = mail;
            _emails = emails;
            _config = config;
        }

        public async Task<PagedResult<ReviewResponse>> GetPagedByRecipeAsync(
            int recipeId, int page, int pageSize,
            string? search, ReviewSortBy sortBy,
            SortOrder sortOrder, ReviewStatus? statusFilter,
            int? currentUserId)
        {
            bool isStaff = false, isOwner = false;
            if (currentUserId.HasValue)
            {
                var u = await _userRepo.GetByIdAsync(currentUserId.Value);
                isStaff = u != null && (u.Role == UserRole.Admin || u.Role == UserRole.Moderator);
                var recipe = await _recipeRepo.GetByIdAsync(recipeId)
                             ?? throw new KeyNotFoundException("Recipe not found");
                isOwner = recipe.UserId == currentUserId.Value;
            }
            var effStatus = (isStaff || isOwner) ? statusFilter : ReviewStatus.Approved;

            var (list, total) = await _repo.GetPagedByRecipeAsync(
                recipeId, page, pageSize,
                search, sortBy, sortOrder,
                effStatus);

            var dtos = list.Select(r => _mapper.Map<ReviewResponse>(r)).ToList();

            return new PagedResult<ReviewResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                Items = dtos
            };
        }

        public async Task<PagedResult<ReviewResponse>> GetMyReviewsAsync(
             int page, int pageSize,
             string? search, ReviewSortBy sortBy,
             SortOrder sortOrder, ReviewStatus? statusFilter,
             int currentUserId)
        {

            var u = await _userRepo.GetByIdAsync(currentUserId)
                    ?? throw new KeyNotFoundException("User not found");

            var (list, total) = await _repo.GetPagedByUserAsync(
                currentUserId, page, pageSize,
                search, sortBy, sortOrder,
                statusFilter);

            var dtos = list.Select(r => _mapper.Map<ReviewResponse>(r)).ToList();
            return new PagedResult<ReviewResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                Items = dtos
            };
        }

        public async Task<ReviewResponse> GetByIdAsync(int id, int? currentUserId)
        {
            var r = await _repo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException($"Review {id} not found");

            bool isStaff = false, isOwner = false;
            if (currentUserId.HasValue)
            {
                var u = await _userRepo.GetByIdAsync(currentUserId.Value);
                isStaff = u != null && (u.Role == UserRole.Admin || u.Role == UserRole.Moderator);
                isOwner = r.UserId == currentUserId.Value;
            }
            if (!isStaff && !isOwner && r.Status != ReviewStatus.Approved)
                throw new UnauthorizedAccessException("Cannot view this review");

            return _mapper.Map<ReviewResponse>(r);
        }

        public async Task<ReviewResponse> CreateAsync(NewReviewRequest dto, int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                       ?? throw new KeyNotFoundException($"User {userId} not found");

            if (user.Status != UserStatus.Approved)
                throw new UnauthorizedAccessException("Only approved users can create reviews.");

            var entity = _mapper.Map<Review>(dto);
            entity.UserId = userId;
            var created = await _repo.AddAsync(entity);

            await RecalculateRecipeAndAuthorStatsAsync(created.RecipeId);

            var recipe = await _recipeRepo.GetByIdAsync(created.RecipeId);
            if (recipe != null)
            {

                var recipeUrl = $"{_config["App:FrontendUrl"]}/recipes/{recipe.Id}";
                var (subject, body) = _emails.BuildReviewCreatedEmail(
                    recipe.Name,
                    created.Reviewer,
                    recipeUrl);
                await _mail.SendAsync(user.Email, subject, body);
            }
            

            return _mapper.Map<ReviewResponse>(created);
        }

        public async Task<ReviewResponse> UpdateAsync(int id, UpdateReviewRequest dto, int currentUserId)
        {
            var existing = await _repo.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Review {id} not found");

            var u = await _userRepo.GetByIdAsync(currentUserId)
                    ?? throw new KeyNotFoundException("User not found");

            if (u.Status != UserStatus.Approved)
                throw new UnauthorizedAccessException("Only approved users can update reviews.");

            bool isStaff = u.Role == UserRole.Admin || (u.Role == UserRole.Moderator);
            bool isOwner = existing.UserId == currentUserId;
            if (!isStaff && !isOwner)
                throw new UnauthorizedAccessException("No permission");

            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);

            await RecalculateRecipeAndAuthorStatsAsync(updated.RecipeId);

            return _mapper.Map<ReviewResponse>(updated);
        }

        public async Task DeleteAsync(int id, int currentUserId)
        {
            var existing = await _repo.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Review {id} not found");

            var u = await _userRepo.GetByIdAsync(currentUserId)
                    ?? throw new KeyNotFoundException("User not found");

            if (u.Status != UserStatus.Approved)
                throw new UnauthorizedAccessException("Only approved users can update reviews.");

            bool isStaff = u.Role == UserRole.Admin || u.Role == UserRole.Moderator;
            bool isOwner = existing.UserId == currentUserId;
            if (!isStaff && !isOwner)
                throw new UnauthorizedAccessException("No permission");

            await _repo.DeleteAsync(id);

            await RecalculateRecipeAndAuthorStatsAsync(existing.RecipeId);
        }

        public async Task<ReviewResponse> ChangeStatusAsync(int id, ReviewStatus newStatus, int currentUserId)
        {
            var r = await _repo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException($"Review {id} not found");
            var u = await _userRepo.GetByIdAsync(currentUserId)
                   ?? throw new KeyNotFoundException("User not found");

            if (!(u.Role == UserRole.Admin || (u.Role == UserRole.Moderator && u.Status == UserStatus.Approved)))
                throw new UnauthorizedAccessException("Only Admins or Approved Moderators can change status");

            r.Status = newStatus;
            var updated = await _repo.UpdateAsync(r);

            await RecalculateRecipeAndAuthorStatsAsync(r.RecipeId);

            var reviewer = await _userRepo.GetByIdAsync(r.UserId);
            if (reviewer != null && reviewer.Status == UserStatus.Approved)
            {
                var reviewUrl = $"{_config["App:FrontendUrl"]}/recipes/{r.RecipeId}#review-{r.Id}";
                var (sub, body) = _emails.BuildReviewStatusChangedEmail(
                    reviewer.UserName,
                    r.Id,
                    newStatus,
                    reviewUrl
                );
                await _mail.SendAsync(reviewer.Email, sub, body);
            }

            return _mapper.Map<ReviewResponse>(updated);
        }


        private async Task RecalculateRecipeAndAuthorStatsAsync(int recipeId)
        {
            var reviews = (await _repo.GetByRecipeIdsAsync(new[] { recipeId })).ToList();

            var recipe = await _recipeRepo.GetByIdAsync(recipeId)
                         ?? throw new KeyNotFoundException($"Recipe {recipeId} not found");

            if (reviews.Any())
            {
                recipe.AvgRating = reviews.Average(r => r.Rating);
                recipe.AvgDifficulty = reviews.Average(r => (decimal)r.Difficulty);
                recipe.RecommendPercentage = reviews.Count(r => r.Recommend) * 100m / reviews.Count;
                recipe.AvgPreparationTimeMinutes = reviews.Average(r => (decimal)r.PreparationTimeMinutes);
                recipe.ReviewsCount = reviews.Count;
            }
            else
            {
                recipe.AvgRating = null;
                recipe.AvgDifficulty = null;
                recipe.RecommendPercentage = null;
                recipe.AvgPreparationTimeMinutes = null;
                recipe.ReviewsCount = null;
            }

            await _recipeRepo.UpdateAsync(recipe);

            var authorId = recipe.UserId;

            var (authorRecipes, _) = await _recipeRepo.GetPagedByUserAsync(
                ownerId: authorId,
                page: 1, pageSize: int.MaxValue,
                search: null,
                sortBy: RecipeSortBy.Name,
                sortOrder: SortOrder.Asc,
                dishType: null,
                statusFilter: null,
                minReviewsCount: null
            );

            var recPercs = authorRecipes
                .Where(r => r.RecommendPercentage.HasValue)
                .Select(r => r.RecommendPercentage!.Value)
                .ToList();

            var newTrust = recPercs.Any()
                ? Math.Round(recPercs.Average(), 2)
                : 0m;

            await _userRepo.SetTrustFactorAsync(authorId, newTrust);
        }

    }
}