using AutoMapper;
using ICookThis.Modules.Reviews.Dtos;
using ICookThis.Modules.Units.Entities;
using ICookThis.Modules.Units.Dtos;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Ingredients.Entities;
using ICookThis.Modules.Ingredients.Dtos;
using ICookThis.Modules.Users.Entities;
using ICookThis.Modules.Users.Dtos;
using ICookThis.Modules.Auth.Dtos;

namespace ICookThis.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ---- Ingredients ----
            CreateMap<NewIngredientRequest, Ingredient>();
            CreateMap<UpdateIngredientRequest, Ingredient>();
            CreateMap<Ingredient, IngredientResponse>();

            // ---- Units ----
            CreateMap<NewUnitRequest, Unit>();
            CreateMap<UpdateUnitRequest, Unit>();
            CreateMap<Unit, UnitResponse>();

            // ---- Recipes ----
            CreateMap<NewRecipeRequest, Recipe>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            CreateMap<UpdateRecipeRequest, Recipe>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            CreateMap<ChangeRecipeStatusRequest, Recipe>();

            CreateMap<Recipe, RecipeResponse>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            CreateMap<RecipeIngredientRequest, RecipeIngredient>();
            CreateMap<RecipeIngredient, RecipeIngredientResponse>();

            CreateMap<NewInstructionStepRequest, InstructionStep>();
            CreateMap<UpdateInstructionStepRequest, InstructionStep>();
            CreateMap<InstructionStep, InstructionStepResponse>();

            // ---- Users ----
            CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<NewUserRequest, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<UpdateUserRequest, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.BannerImage, opt => opt.Ignore())
                .ForMember(dest => dest.TrustFactor, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewTrustFactor, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<User, UserResponse>();

            CreateMap<User, PublicUserResponse>()
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.BannerImage, opt => opt.MapFrom(s => s.BannerImage))
                .ForMember(d => d.TrustFactor, opt => opt.MapFrom(s => s.TrustFactor))
                .ForMember(d => d.ReviewTrustFactor, opt => opt.MapFrom(s => s.ReviewTrustFactor));

            CreateMap<RegisterRequest, User>();
            CreateMap<User, AuthResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.Expires, opt => opt.Ignore());

            CreateMap<StepIngredientRequest, StepIngredient>();
            CreateMap<StepIngredient, StepIngredientResponse>();

            // ---- ReviewVotes ----
            CreateMap<ReviewVote, ReviewVoteDto>();
            CreateMap<NewReviewVoteRequest, ReviewVote>();

            // ---- Reviews ----
            CreateMap<NewReviewRequest, Review>();

            CreateMap<UpdateReviewRequest, Review>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember, context) => srcMember != null));

            CreateMap<Review, ReviewResponse>();
        }
    }
}
