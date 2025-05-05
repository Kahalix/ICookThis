using ICookThis.Modules.Ingredients.Entities;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Modules.Units.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;

namespace ICookThis.Data
{
    public class CookThisDbContext : DbContext
    {
        public CookThisDbContext(DbContextOptions<CookThisDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<InstructionStep> InstructionSteps { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<StepIngredient> StepIngredients { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Unit> Units { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enums as int
            modelBuilder.Entity<Recipe>()
                .Property(r => r.DishType)
                .HasConversion<int>();

            modelBuilder.Entity<Unit>()
                .Property(u => u.Type)
                .HasConversion<int>();

            // Precyzje dla decimal
            modelBuilder.Entity<StepIngredient>()
                .Property(si => si.Fraction)
                .HasPrecision(5, 4);

            modelBuilder.Entity<Review>()
                .Property(r => r.Rating)
                .HasPrecision(2, 1);

            modelBuilder.Entity<Recipe>()
                .Property(r => r.AvgRating)
                .HasPrecision(3, 2);
            modelBuilder.Entity<Recipe>()
                .Property(r => r.AvgDifficulty)
                .HasPrecision(3, 2);
            modelBuilder.Entity<Recipe>()
                .Property(r => r.RecommendPercentage)
                .HasPrecision(5, 2);

            // Relacje (bez nawigacyjnych kolekcji po stronie 'one')
            modelBuilder.Entity<InstructionStep>()
                .HasOne<Recipe>()
                .WithMany()
                .HasForeignKey(s => s.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recipe>()
                .HasOne<Unit>()
                .WithMany()
                .HasForeignKey(r => r.DefaultUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne<Recipe>()
                .WithMany()
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne<Ingredient>()
                .WithMany()
                .HasForeignKey(ri => ri.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne<Unit>()
                .WithMany()
                .HasForeignKey(ri => ri.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StepIngredient>()
                .HasOne<InstructionStep>()
                .WithMany()
                .HasForeignKey(si => si.InstructionStepId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StepIngredient>()
                .HasOne<Ingredient>()
                .WithMany()
                .HasForeignKey(si => si.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne<Recipe>()
                .WithMany()
                .HasForeignKey(rv => rv.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1) Unikalność nazw
            modelBuilder.Entity<Ingredient>()
                .HasIndex(i => i.Name)
                .IsUnique();
            modelBuilder.Entity<Unit>()
                .HasIndex(u => u.Symbol)
                .IsUnique();
            modelBuilder.Entity<Recipe>()
                .HasIndex(r => new { r.Name, r.DishType })
                .IsUnique();

            // 2) Check-constraints na wartości logiczne

            modelBuilder.Entity<Unit>()
                .ToTable(tb => tb.HasCheckConstraint(
                    "CK_Unit_Type_Range",
                    "[Type] >= 0 AND [Type] <= 2"));

            modelBuilder.Entity<Recipe>().
                ToTable(tb => tb.HasCheckConstraint(
                    "CK_Review_DishType_Range",
                    "[DishType] >= 0 AND [DishType] <= 4"));

            modelBuilder.Entity<Review>()
                .ToTable(tb => tb.HasCheckConstraint(
                "CK_Review_Difficulty_Range",
                "[Difficulty] >= 1 AND [Difficulty] <= 5"));

            modelBuilder.Entity<Review>()
                .ToTable(tb => tb.HasCheckConstraint(
                "CK_Review_PreparationTime_Range",
                "[PreparationTimeMinutes] >= 1"));

            modelBuilder.Entity<Recipe>()
                .ToTable(tb => tb.HasCheckConstraint(
                "CK_Recipe_DefaultQty_Range",
                "[DefaultQty] > 0"));

            modelBuilder.Entity<RecipeIngredient>(entity =>
            {
                entity.ToTable(tb => tb.HasCheckConstraint(
                    "CK_RecipeIngredient_Qty_Positive",
                    "[Qty] > 0"));
            });

            modelBuilder.Entity<StepIngredient>(entity =>
            {
                entity.ToTable(tb => tb.HasCheckConstraint(
                    "CK_StepIngredient_Fraction_Range",
                    "[Fraction] >= 0 AND [Fraction] <= 1"));
            });

            modelBuilder.Entity<Review>()
                .ToTable(tb => tb.HasCheckConstraint(
                "CK_Review_Rating_Range",
                "[Rating] >= 1 AND [Rating] <= 5"));

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable(tb => tb.HasCheckConstraint(
                    "CK_Recipe_DefaultQty_Positive",
                    "[DefaultQty] > 0"));
            });



            // 3) Unikalny StepOrder w obrębie przepisu
            modelBuilder.Entity<InstructionStep>()
                .HasIndex(s => new { s.RecipeId, s.StepOrder })
                .IsUnique();

        }
    }
}
