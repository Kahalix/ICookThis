using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Modules.Users.Entities;

namespace ICookThis.Utils.Email
{
    public interface IEmailBuilder
    {
        (string Subject, string BodyHtml) BuildConfirmationEmail(string userName, string confirmUrl);
        (string Subject, string BodyHtml) BuildPasswordResetEmail(string userName, string resetUrl);
        (string Subject, string BodyHtml) BuildAdminCreatedUserEmail(string userName);
        (string Subject, string BodyHtml) BuildStatusChangedEmail(string userName, UserStatus newStatus);
        (string Subject, string BodyHtml) BuildRoleChangedEmail(string userName, UserRole newRole);
        (string Subject, string BodyHtml) BuildReviewCreatedEmail(
            string recipeName, string reviewerName, string recipeUrl);
        (string Subject, string BodyHtml) BuildReviewStatusChangedEmail(
            string reviewerName, int reviewId, ReviewStatus newStatus, string reviewUrl);
        (string Subject, string BodyHtml) BuildRecipeCreatedEmail(
            string userName, string recipeName, string recipeUrl);
        (string Subject, string BodyHtml) BuildRecipeStatusChangedEmail(
            string userName, string recipeName, RecipeStatus newStatus, string recipeUrl);
    }
}