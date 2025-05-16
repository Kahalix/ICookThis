using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ICookThis.Modules.Ingredients.Dtos;
using ICookThis.Modules.Ingredients.Repositories;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Recipes.Repositories;
using ICookThis.Modules.Units.Dtos;
using ICookThis.Modules.Units.Repositories;
using ICookThis.Modules.Users.Entities;
using ICookThis.Modules.Users.Repositories;
using ICookThis.Shared.Dtos;
using ICookThis.Utils;
using ICookThis.Utils.Email;

namespace ICookThis.Modules.Recipes.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IUserRepository _userRepo;
        private readonly IRecipeIngredientRepository _riRepo;
        private readonly IInstructionStepRepository _stepRepo;
        private readonly IStepIngredientRepository _siRepo;
        private readonly IMapper _mapper;
        private readonly IIngredientRepository _ingredientRepo;
        private readonly IUnitRepository _unitRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMailService _mail;
        private readonly IEmailBuilder _emails;
        private readonly IConfiguration _config;

        public RecipeService(
            IRecipeRepository recipeRepo,
            IUserRepository userRepo,
            IRecipeIngredientRepository riRepo,
            IInstructionStepRepository stepRepo,
            IStepIngredientRepository siRepo,
            IIngredientRepository ingredientRepo,
            IUnitRepository unitRepo,
            IMapper mapper,
            IWebHostEnvironment env,
            IMailService mail,
            IEmailBuilder emails,
            IConfiguration config)
        {
            _recipeRepo = recipeRepo;
            _userRepo = userRepo;
            _riRepo = riRepo;
            _stepRepo = stepRepo;
            _siRepo = siRepo;
            _ingredientRepo = ingredientRepo;
            _unitRepo = unitRepo;
            _mapper = mapper;
            _env = env;
            _mail = mail;
            _emails = emails;
            _config = config;
        }
        public async Task<PagedResult<RecipeListResponse>> GetPagedAsync(
            int page, int pageSize, string? search,
            RecipeSortBy sortBy, SortOrder sortOrder,
            DishType? dishType,
            RecipeStatus? statusFilter,
            int? minReviewsCount,
            int? userId)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            bool isStaff = false;
            if (userId.HasValue)
            {
                var u = await _userRepo.GetByIdAsync(userId.Value);
                isStaff = u != null && (u.Role == UserRole.Admin || u.Role == UserRole.Moderator);
            }

            var effectiveStatus = isStaff ? statusFilter : RecipeStatus.Approved;

            var (recipes, total) = await _recipeRepo.GetPagedAsync(
                page, pageSize,
                search, sortBy, sortOrder,
                dishType, effectiveStatus, minReviewsCount);

            var userIds = recipes.Select(r => r.UserId).Distinct().ToList();
            var users = (await _userRepo.GetByIdsAsync(userIds))
                            .ToDictionary(u => u.Id);

            var items = recipes.Select(r =>
            {
                var summary = new RecipeListResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    DishType = r.DishType,
                    Status = r.Status,
                    ReviewsCount = r.ReviewsCount,
                    AvgRating = r.AvgRating,
                    UserName = users[r.UserId].UserName,
                    TrustFactor = users[r.UserId].TrustFactor
                };
                return summary;
            }).ToList();

            return new PagedResult<RecipeListResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)System.Math.Ceiling(total / (double)pageSize),
                Items = items,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Search = search
            };
        }


        public async Task<PagedResult<RecipeListResponse>> GetByUserAsync(
             int ownerId,
             int page,
             int pageSize,
             string? search,
             RecipeSortBy sortBy,
             SortOrder sortOrder,
             DishType? dishType,
             RecipeStatus? statusFilter,
             int? minReviewsCount,
             int? currentUserId)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            bool isStaff = false, isOwner = false;
            if (currentUserId.HasValue)
            {
                var cu = await _userRepo.GetByIdAsync(currentUserId.Value);
                isStaff = cu != null && (cu.Role == UserRole.Admin || cu.Role == UserRole.Moderator);
                isOwner = cu != null && cu.Id == ownerId;
            }

            var effectiveStatus = (isStaff || isOwner)
                ? statusFilter
                : RecipeStatus.Approved;

            var (recipes, total) = await _recipeRepo.GetPagedByUserAsync(
                ownerId, page, pageSize,
                search,
                sortBy, sortOrder,
                dishType, effectiveStatus,
                minReviewsCount);

            var author = await _userRepo.GetByIdAsync(ownerId)
                         ?? throw new KeyNotFoundException($"User {ownerId} not found");

            var items = recipes.Select(r => new RecipeListResponse
            {
                Id = r.Id,
                Name = r.Name,
                DishType = r.DishType,
                Status = r.Status,
                ReviewsCount = r.ReviewsCount,
                AvgRating = r.AvgRating,
                UserName = author.UserName,
                TrustFactor = author.TrustFactor
            }).ToList();

            return new PagedResult<RecipeListResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                Items = items,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Search = search
            };
        }

        public async Task<PagedResult<RecipeListResponse>> GetMyRecipesAsync(
            int page,
            int pageSize,
            string? search,
            RecipeSortBy sortBy,
            SortOrder sortOrder,
            DishType? dishType,
            RecipeStatus? statusFilter,
            int? minReviewsCount,
            int currentUserId)
        {
            return await GetByUserAsync(
                ownerId: currentUserId,
                page: page,
                pageSize: pageSize,
                search: search,
                sortBy: sortBy,
                sortOrder: sortOrder,
                dishType: dishType,
                statusFilter: statusFilter,
                minReviewsCount: minReviewsCount,
                currentUserId: currentUserId);
        }

        public async Task<RecipeResponse> GetByIdAsync(int id, int? userId, decimal? scale = null)
        {
            var recipe = await _recipeRepo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Recipe {id} not found");

            bool isOwner = userId.HasValue && recipe.UserId == userId.Value;

            bool isStaff = false;
            if (userId.HasValue)
            {
                var user = await _userRepo.GetByIdAsync(userId.Value);
                isStaff = user != null &&
                          (user.Role == UserRole.Admin || user.Role == UserRole.Moderator);
            }

            if (!isOwner && !isStaff && recipe.Status != RecipeStatus.Approved)
                throw new UnauthorizedAccessException("You cannot view this recipe");

            var dto = _mapper.Map<RecipeResponse>(recipe);
            var defaultUnit = await _unitRepo.GetByIdAsync(recipe.DefaultUnitId);
            dto.DefaultUnit = _mapper.Map<UnitResponse>(defaultUnit);

            var ris = (await _riRepo.GetByRecipeIdsAsync(new[] { id })).ToList();
            var ingredientIds = ris.Select(ri => ri.IngredientId).Distinct();
            var unitIds = ris.Select(ri => ri.UnitId).Distinct();
            var allIngredients = (await _ingredientRepo.GetByIdsAsync(ingredientIds))
                                 .ToDictionary(i => i.Id);
            var allUnits = (await _unitRepo.GetByIdsAsync(unitIds))
                                 .ToDictionary(u => u.Id);

            var owner = await _userRepo.GetByIdAsync(recipe.UserId)
                ?? throw new KeyNotFoundException($"User {recipe.UserId} not found");
            dto.UserName = owner.UserName;
            dto.TrustFactor = owner.TrustFactor;

            dto.Ingredients = ris.Select(ri => new RecipeIngredientResponse
            {
                Id = ri.Id,
                RecipeId = ri.RecipeId,
                Ingredient = _mapper.Map<IngredientResponse>(allIngredients[ri.IngredientId]),
                Qty = ri.Qty,
                Unit = _mapper.Map<UnitResponse>(allUnits[ri.UnitId])
            }).ToList();

            var steps = (await _stepRepo.GetByRecipeIdsAsync(new[] { id }))
                          .OrderBy(s => s.StepOrder).ToList();
            var stepIds = steps.Select(s => s.Id).ToList();
            var sis = (await _siRepo.GetByStepIdsAsync(stepIds)).ToList();
            var siIngredientIds = sis.Select(si => si.IngredientId).Distinct();
            var allSiIngredients = (await _ingredientRepo.GetByIdsAsync(siIngredientIds))
                                    .ToDictionary(i => i.Id);

            dto.Steps = new List<InstructionStepResponse>();
            foreach (var step in steps)
            {
                var stepDto = _mapper.Map<InstructionStepResponse>(step);
                var relatedSis = sis.Where(si => si.InstructionStepId == step.Id);
                stepDto.StepIngredients = relatedSis.Select(si => new StepIngredientResponse
                {
                    Id = si.Id,
                    InstructionStepId = si.InstructionStepId,
                    Ingredient = _mapper.Map<IngredientResponse>(allSiIngredients[si.IngredientId]),
                    Fraction = si.Fraction
                }).ToList();

                stepDto.Text = step.TemplateText;
                if (scale.HasValue)
                {
                    stepDto.Text = PlaceholderReplacer.Replace(
                        step.TemplateText,
                        dto.Ingredients,
                        stepDto.StepIngredients,
                        scale.Value);
                }
                dto.Steps.Add(stepDto);
            }

            if (scale.HasValue)
            {
                var s = scale.Value;
                dto.DefaultQty *= s;
                foreach (var ri in dto.Ingredients)
                    ri.Qty *= s;
            }

            return dto;
        }

        public async Task<RecipeResponse> CreateAsync(NewRecipeRequest request, int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                       ?? throw new KeyNotFoundException($"User {userId} not found");

            if (user.Status != UserStatus.Approved)
                throw new UnauthorizedAccessException("Only approved users can create recipes.");

            var entity = _mapper.Map<Recipe>(request);
            entity.UserId = userId;

            if (user.Role == UserRole.Admin || user.Role == UserRole.Moderator)
            {
                entity.AddedBy = AddedBy.Staff;
                entity.Status = RecipeStatus.Approved;
            }
            else
            {
                entity.AddedBy = AddedBy.User;
                entity.Status = RecipeStatus.Pending;
            }

            if (request.ImageFile != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images", "recipes");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ImageFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using var stream = System.IO.File.Create(filePath);
                await request.ImageFile.CopyToAsync(stream);
                entity.Image = Path.Combine("images", "recipes", fileName).Replace("\\", "/");
            }

            var created = await _recipeRepo.AddAsync(entity);

            foreach (var dto in request.Ingredients)
            {
                var ri = _mapper.Map<RecipeIngredient>(dto);
                ri.RecipeId = created.Id;
                await _riRepo.AddAsync(ri);
            }

            foreach (var stepDto in request.Steps)
            {
                if (stepDto.ImageFile != null)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "images", "instructionsteps");
                    Directory.CreateDirectory(uploads);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(stepDto.ImageFile.FileName)}";
                    var filePath = Path.Combine(uploads, fileName);
                    using var s = System.IO.File.Create(filePath);
                    await stepDto.ImageFile.CopyToAsync(s);
                    stepDto.Image = Path.Combine("images", "instructionsteps", fileName).Replace("\\", "/");
                }

                var step = _mapper.Map<InstructionStep>(stepDto);
                step.RecipeId = created.Id;
                var addedStep = await _stepRepo.AddAsync(step);

                foreach (var siDto in stepDto.StepIngredients ?? Enumerable.Empty<StepIngredientRequest>())
                {
                    var si = _mapper.Map<StepIngredient>(siDto);
                    si.InstructionStepId = addedStep.Id;
                    await _siRepo.AddAsync(si);
                }
            }


            var recipeUrl = $"{_config["App:FrontendUrl"]}/recipes/{created.Id}";
            var (sub, body) = _emails.BuildRecipeCreatedEmail(
                user.UserName, created.Name, recipeUrl);
            await _mail.SendAsync(user.Email, sub, body);

            return await GetByIdAsync(created.Id, userId, null);
        }

        public async Task<RecipeResponse> UpdateAsync(int id, UpdateRecipeRequest request, int userId)
        {
            var existing = await _recipeRepo.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Recipe {id} not found");

            var user = await _userRepo.GetByIdAsync(userId)
                       ?? throw new KeyNotFoundException($"User {userId} not found");

            if (user.Status != UserStatus.Approved)
                throw new UnauthorizedAccessException("Only approved users can create recipes.");

            bool isOwner = existing.UserId == userId;
            bool isStaff = user.Role == UserRole.Admin || user.Role == UserRole.Moderator;
            if (!isOwner && !isStaff)
                throw new UnauthorizedAccessException("You do not have permission to edit this recipe");

            if ((request.RemoveImage || request.ImageFile != null)
                && !string.IsNullOrEmpty(existing.Image))
            {
                var toDelete = Path.Combine(
                    _env.WebRootPath,
                    existing.Image.Replace("/", Path.DirectorySeparatorChar.ToString())
                );
                if (File.Exists(toDelete))
                    File.Delete(toDelete);
            }

            if (request.ImageFile != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images", "recipes");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ImageFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using var fs = File.Create(filePath);
                await request.ImageFile.CopyToAsync(fs);
                request.Image = Path.Combine("images", "recipes", fileName).Replace("\\", "/");
            }
            else if (request.RemoveImage)
            {
                request.Image = null;
            }

            _mapper.Map(request, existing);
            await _recipeRepo.UpdateAsync(existing);

            var oldRis = await _riRepo.GetByRecipeAsync(id);
            foreach (var old in oldRis)
                await _riRepo.DeleteAsync(old.Id);

            foreach (var dto in request.Ingredients)
            {
                var ri = _mapper.Map<RecipeIngredient>(dto);
                ri.RecipeId = id;
                await _riRepo.AddAsync(ri);
            }

            var oldSteps = await _stepRepo.GetByRecipeAsync(id);
            foreach (var os in oldSteps)
            {
                if (!string.IsNullOrEmpty(os.Image))
                {
                    var p = Path.Combine(_env.WebRootPath, os.Image.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (File.Exists(p)) File.Delete(p);
                }
                var oldSis = await _siRepo.GetByStepAsync(os.Id);
                foreach (var osi in oldSis)
                    await _siRepo.DeleteAsync(osi.Id);
                await _stepRepo.DeleteAsync(os.Id);
            }

            foreach (var stepDto in request.Steps)
            {
                if (stepDto.ImageFile != null)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "images", "instructionsteps");
                    Directory.CreateDirectory(uploads);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(stepDto.ImageFile.FileName)}";
                    var filePath = Path.Combine(uploads, fileName);
                    using var fs = File.Create(filePath);
                    await stepDto.ImageFile.CopyToAsync(fs);
                    stepDto.Image = Path.Combine("images", "instructionsteps", fileName).Replace("\\", "/");
                }
                else if (stepDto.RemoveImage)
                {
                    stepDto.Image = null;
                }

                var stepEntity = _mapper.Map<InstructionStep>(stepDto);
                stepEntity.RecipeId = id;
                var addedStep = await _stepRepo.AddAsync(stepEntity);

                foreach (var siDto in stepDto.StepIngredients ?? Enumerable.Empty<StepIngredientRequest>())
                {
                    var si = _mapper.Map<StepIngredient>(siDto);
                    si.InstructionStepId = addedStep.Id;
                    await _siRepo.AddAsync(si);
                }
            }

            return await GetByIdAsync(id, userId, null);
        }

        public async Task<RecipeResponse> ChangeStatusAsync(int recipeId, RecipeStatus newStatus, int userId)
        {
            var recipe = await _recipeRepo.GetByIdAsync(recipeId)
                         ?? throw new KeyNotFoundException($"Recipe {recipeId} not found");

            var user = await _userRepo.GetByIdAsync(userId)
                       ?? throw new KeyNotFoundException($"User {userId} not found");

            if (user.Status != UserStatus.Approved)
                throw new UnauthorizedAccessException("Only approved users can update reviews.");

            bool isStaff = user.Role == UserRole.Admin || user.Role == UserRole.Moderator;
            bool isOwner = recipe.UserId == userId;

            if (isStaff)
            {
                recipe.Status = newStatus;
            }
            else if (isOwner)
            {
                if (newStatus == RecipeStatus.Archived || newStatus == RecipeStatus.Hidden || newStatus == RecipeStatus.Pending)
                    recipe.Status = newStatus;
                else
                    throw new UnauthorizedAccessException(
                        "As the owner you can only archive, hide or publish your recipe to be verified");
            }
            else
            {
                throw new UnauthorizedAccessException(
                    "You do not have permission to change the status of this recipe");
            }

            await _recipeRepo.UpdateAsync(recipe);

            if (user?.Role == UserRole.Admin || user?.Role == UserRole.Moderator)
            {
                var author = await _userRepo.GetByIdAsync(recipe.UserId);
                if (author != null && author.Status == UserStatus.Approved)
                {
                    var recipeUrl = $"{_config["App:FrontendUrl"]}/recipes/{recipeId}";
                    var (sub, body) = _emails.BuildRecipeStatusChangedEmail(
                        author.UserName, recipe.Name, newStatus, recipeUrl
                    );
                    await _mail.SendAsync(author.Email, sub, body);
                }
            }

            return await GetByIdAsync(recipeId, userId, null);
        }

        public async Task DeleteAsync(int id)
        {
            var recipe = await _recipeRepo.GetByIdAsync(id);
            if (recipe == null) return;

            if (!string.IsNullOrEmpty(recipe.Image)
                && !recipe.Image.EndsWith("default.jpg", StringComparison.OrdinalIgnoreCase))
            {
                var fullPath = Path.Combine(
                    _env.WebRootPath,
                    recipe.Image.Replace("/", Path.DirectorySeparatorChar.ToString())
                );
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }

            var steps = await _stepRepo.GetByRecipeAsync(id);
            foreach (var step in steps)
            {
                if (!string.IsNullOrEmpty(step.Image)
                    && !step.Image.EndsWith("default.jpg", StringComparison.OrdinalIgnoreCase))
                {
                    var stepPath = Path.Combine(
                        _env.WebRootPath,
                        step.Image.Replace("/", Path.DirectorySeparatorChar.ToString())
                    );
                    if (File.Exists(stepPath))
                        File.Delete(stepPath);
                }
            }

            await _recipeRepo.DeleteAsync(id);
        }
    }
}