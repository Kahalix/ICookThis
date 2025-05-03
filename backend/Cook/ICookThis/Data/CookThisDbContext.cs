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

            // Enumy jako int
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

        }
    }
}
