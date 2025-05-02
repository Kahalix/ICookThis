using Microsoft.EntityFrameworkCore;
using YourApp.Modules.Ingredients.Entities;
using YourApp.Modules.Recipes.Entities;
using YourApp.Modules.Reviews.Entities;
using YourApp.Modules.Units.Entities;

namespace YourApp.Data
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
            // Enumy jako stringi lub inty (domyślnie inty)
            modelBuilder.Entity<Recipe>()
                .Property(r => r.DishType)
                .HasConversion<int>();

            modelBuilder.Entity<Unit>()
                .Property(u => u.Type)
                .HasConversion<int>();

            // Relacje
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

            base.OnModelCreating(modelBuilder);
        }
    }
}
