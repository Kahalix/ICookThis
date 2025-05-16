using System.Text;

namespace ICookThis.Shared.Helpers
{
    public static class EmailTemplateBuilder
    {
        public static string Wrap(string title, string bodyContent)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"UTF-8\" />");
            sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />");
            sb.AppendLine($"  <title>{title}</title>");
            sb.AppendLine("  <style>");
            sb.AppendLine("    body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin:0; padding:0; }");
            sb.AppendLine("    .container { max-width:600px; margin:20px auto; background:#fff; padding:20px; border-radius:8px; }");
            sb.AppendLine("    h1 { color:#333; font-size:24px; margin-bottom:10px; }");
            sb.AppendLine("    p { color:#555; line-height:1.5; }");
            sb.AppendLine("    a.button { display:inline-block; padding:10px 20px; margin:10px 0; background:#28a745; color:#fff; text-decoration:none; border-radius:4px; }");
            sb.AppendLine("    .footer { font-size:12px; color:#999; margin-top:20px; text-align:center; }");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("  <div class=\"container\">");
            sb.AppendLine($"    <h1>{title}</h1>");
            sb.AppendLine($"    {bodyContent}");
            sb.AppendLine("    <div class=\"footer\">");
            sb.AppendLine("      <p>If you didn’t request this, you can safely ignore this email.</p>");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }
    }
}
