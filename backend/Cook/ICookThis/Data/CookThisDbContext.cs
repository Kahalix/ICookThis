using ICookThis.Modules.Auth.Entities;
using ICookThis.Modules.Ingredients.Entities;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Modules.Units.Entities;
using ICookThis.Modules.Users.Entities;
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
        public DbSet<ReviewVote> ReviewVotes { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }

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

            modelBuilder.Entity<Recipe>()
              .Property(r => r.Status)
              .HasConversion<int>();

            modelBuilder.Entity<Recipe>()
              .Property(r => r.AddedBy)
              .HasConversion<int>();

            modelBuilder.Entity<Review>()
              .Property(r => r.Status)
              .HasConversion<int>();

            modelBuilder.Entity<User>()
              .Property(u => u.Status)
              .HasConversion<int>();

            modelBuilder.Entity<User>()
              .Property(u => u.Role)
              .HasConversion<int>();

            // Decimal precisions
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
            modelBuilder.Entity<User>()
                .Property(u => u.TrustFactor)
                .HasPrecision(5, 2);
            modelBuilder.Entity<User>()
                .Property(u => u.ReviewTrustFactor)
                .HasPrecision(5, 2);
            modelBuilder.Entity<Recipe>()
                .Property(r => r.DefaultQty)
                .HasPrecision(9, 3);

            // ReviewVote: composite PK
            modelBuilder.Entity<ReviewVote>()
              .HasKey(rv => new { rv.ReviewId, rv.UserId });

            modelBuilder.Entity<ReviewVote>()
              .HasOne<Review>()
              .WithMany()
              .HasForeignKey(rv => rv.ReviewId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReviewVote>()
              .HasOne<User>()
              .WithMany()
              .HasForeignKey(rv => rv.UserId)
              .OnDelete(DeleteBehavior.Restrict);

            // --- Recipe -> User (AddedByUserId)
            modelBuilder.Entity<Recipe>()
              .HasOne<User>()
              .WithMany()
              .HasForeignKey(r => r.UserId)
              .OnDelete(DeleteBehavior.Restrict);

            // --- Review -> User (Reviewer)
            modelBuilder.Entity<Review>()
              .HasOne<User>()
              .WithMany()
              .HasForeignKey(r => r.UserId)
              .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<UserToken>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ingredient>()
                .HasIndex(i => i.Name)
                .IsUnique();
            modelBuilder.Entity<Unit>()
                .HasIndex(u => u.Symbol)
                .IsUnique();
            modelBuilder.Entity<Recipe>()
                .HasIndex(r => new { r.Name, r.DishType })
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique()
                .HasFilter("[PhoneNumber] IS NOT NULL");
            modelBuilder.Entity<ReviewVote>()
                .HasIndex(rv => new { rv.ReviewId, rv.UserId })
                .IsUnique();
            modelBuilder.Entity<Review>()
                .HasIndex(rv => new { rv.RecipeId, rv.UserId })
                .IsUnique();

            modelBuilder.Entity<User>()
                .ToTable(tb => tb.HasCheckConstraint(
                    "CK_User_Password_Length",
                    "LEN([Password]) >= 8"));
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


            modelBuilder.Entity<InstructionStep>()
                .HasIndex(s => new { s.RecipeId, s.StepOrder })
                .IsUnique();


        }
    }
}
