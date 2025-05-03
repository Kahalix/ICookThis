// Infrastructure/Mapping/AutoMapperProfile.cs
using AutoMapper;
using ICookThis.Modules.Reviews.Dtos;
using ICookThis.Modules.Units.Entities;
using ICookThis.Modules.Units.Dtos;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Ingredients.Entities;
using ICookThis.Modules.Ingredients.Dtos;

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
            CreateMap<NewRecipeRequest, Recipe>();
            CreateMap<UpdateRecipeRequest, Recipe>();
            CreateMap<Recipe, RecipeResponse>();

            CreateMap<RecipeIngredientRequest, RecipeIngredient>();
            CreateMap<RecipeIngredient, RecipeIngredientResponse>();

            CreateMap<InstructionStepRequest, InstructionStep>();
            CreateMap<InstructionStep, InstructionStepResponse>();

            CreateMap<StepIngredientRequest, StepIngredient>();
            CreateMap<StepIngredient, StepIngredientResponse>();

            // ---- Reviews ----
            CreateMap<NewReviewRequest, Review>();
            CreateMap<UpdateReviewRequest, Review>()
                // umożliwia mapowanie nulli, żeby nie nadpisywać
                .ForAllMembers(opt => opt.Condition((src, dest, _, _) => src != null));
            CreateMap<Review, ReviewResponse>();
        }
    }
}
