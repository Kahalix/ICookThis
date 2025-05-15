namespace ICookThis.Utils.Email
{
    public class EmailBuilder : IEmailBuilder
    {
        public (string Subject, string BodyHtml) BuildConfirmationEmail(string confirmUrl)
        {
            var subject = "Confirm your account";
            var body = $@"
                <p>Thank you for registering!</p>
                <p>Please confirm your email by clicking <a href=""{confirmUrl}"">this link</a>.</p>
                <p>This link will expire in 14 days.</p>";
            return (subject, body);
        }

        public (string Subject, string BodyHtml) BuildPasswordResetEmail(string resetUrl)
        {
            var subject = "Reset your password";
            var body = $@"
                <p>You requested a password reset.</p>
                <p>Click <a href=""{resetUrl}"">here</a> to set a new password.</p>
                <p>This link will expire in 1 hour.</p>";
            return (subject, body);
        }
    }
}
