using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Modules.Users.Entities;
using ICookThis.Shared.Helpers;

namespace ICookThis.Utils.Email
{
    public class EmailBuilder : IEmailBuilder
    {
        public (string Subject, string BodyHtml) BuildConfirmationEmail(string userName, string confirmUrl)
        {
            var subject = "Confirm your ICookThis account";
            var inner = $@"
                <p>Hi <strong>{userName}</strong>,</p>
                <p>Thank you for signing up on ICookThis!</p>
                <p>Please confirm your email by clicking the button below:</p>
                <p style=""text-align:center;"">
                  <a class=""button"" href=""{confirmUrl}"">Confirm Email</a>
                </p>
                <p>This link will expire in 14 days.</p>";
            var body = EmailTemplateBuilder.Wrap(subject, inner);
            return (subject, body);
        }

        public (string Subject, string BodyHtml) BuildPasswordResetEmail(string userName, string resetUrl)
        {
            var subject = "Reset your ICookThis password";
            var inner = $@"
                <p>Hi <strong>{userName}</strong>,</p>
                <p>We received a request to reset your password.</p>
                <p>Please click the button below to choose a new one:</p>
                <p style=""text-align:center;"">
                  <a class=""button"" href=""{resetUrl}"">Reset Password</a>
                </p>
                <p>This link will expire in 1 hour.</p>";
            var body = EmailTemplateBuilder.Wrap(subject, inner);
            return (subject, body);
        }

        public (string Subject, string BodyHtml) BuildAdminCreatedUserEmail(string userName)
        {
            var subject = "Your ICookThis account has been created";
            var inner = $@"
                <p>Hi <strong>{userName}</strong>,</p>
                <p>An administrator has created an ICookThis account for you.</p>
                <p>Please log in to complete your profile setup.</p>";
            var body = EmailTemplateBuilder.Wrap(subject, inner);
            return (subject, body);
        }

        public (string Subject, string BodyHtml) BuildStatusChangedEmail(string userName, UserStatus newStatus)
        {
            var subject = "Your ICookThis account status has been updated";
            var inner = $@"
                <p>Hi <strong>{userName}</strong>,</p>
                <p>Your account has been <strong>{newStatus}</strong>.</p>
                <p>If you have any questions, feel free to contact support.</p>";
            var body = EmailTemplateBuilder.Wrap(subject, inner);
            return (subject, body);
        }

        public (string Subject, string BodyHtml) BuildRoleChangedEmail(string userName, UserRole newRole)
        {
            var subject = "Your ICookThis user role has been updated";
            var inner = $@"
                <p>Hi <strong>{userName}</strong>,</p>
                <p>Your user role has been changed to: <strong>{newRole}</strong>.</p>
                <p>If you have any questions, feel free to reach out to the administrators.</p>";
            var body = EmailTemplateBuilder.Wrap(subject, inner);
            return (subject, body);
        }

        public (string Subject, string BodyHtml) BuildReviewCreatedEmail(
            string recipeName, string reviewerName, string recipeUrl)
        {
            var subject = $"New review on your recipe “{recipeName}”";
            var inner = $@"
                <p>Hi,</p>
                <p>Your recipe <strong>{recipeName}</strong> has just received a new review by <strong>{reviewerName}</strong>.</p>
                <p>You can read it by clicking the button below:</p>
                <p style=""text-align:center;"">
                  <a class=""button"" href=""{recipeUrl}"">View Recipe</a>
                </p>";
            return (subject, EmailTemplateBuilder.Wrap(subject, inner));
        }

        public (string Subject, string BodyHtml) BuildReviewStatusChangedEmail(
            string reviewerName, int reviewId, ReviewStatus newStatus, string reviewUrl)
        {
            var subject = $"Your review #{reviewId} status updated to {newStatus}";
            var inner = $@"
                <p>Hi <strong>{reviewerName}</strong>,</p>
                <p>The status of your review (ID: {reviewId}) has been changed to <strong>{newStatus}</strong>.</p>
                <p>You can view it here:</p>
                <p style=""text-align:center;"">
                  <a class=""button"" href=""{reviewUrl}"">View Review</a>
                </p>";
            return (subject, EmailTemplateBuilder.Wrap(subject, inner));
        }

        public (string Subject, string BodyHtml) BuildRecipeCreatedEmail(
            string userName, string recipeName, string recipeUrl)
        {
            var subject = "Your recipe has been created!";
            var inner = $@"
                <p>Hi <strong>{userName}</strong>,</p>
                <p>Your recipe <strong>{recipeName}</strong> has just been created.</p>
                <p>View it here:</p>
                <p style=""text-align:center;"">
                  <a class=""button"" href=""{recipeUrl}"">See your recipe</a>
                </p>";
            return (subject, EmailTemplateBuilder.Wrap(subject, inner));
        }

        public (string Subject, string BodyHtml) BuildRecipeStatusChangedEmail(
            string userName, string recipeName, RecipeStatus newStatus, string recipeUrl)
        {
            var subject = $"Your recipe status is now {newStatus}";
            var inner = $@"
                <p>Hi <strong>{userName}</strong>,</p>
                <p>The status of your recipe <strong>{recipeName}</strong> has been changed to <strong>{newStatus}</strong>.</p>
                <p>View it here:</p>
                <p style=""text-align:center;"">
                  <a class=""button"" href=""{recipeUrl}"">View recipe</a>
                </p>";
            return (subject, EmailTemplateBuilder.Wrap(subject, inner));
        }
    }
}