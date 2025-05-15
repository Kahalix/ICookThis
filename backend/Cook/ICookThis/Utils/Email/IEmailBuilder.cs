namespace ICookThis.Utils.Email
{
    public interface IEmailBuilder
    {
        (string Subject, string BodyHtml) BuildConfirmationEmail(string confirmUrl);
        (string Subject, string BodyHtml) BuildPasswordResetEmail(string resetUrl);
    }
}
