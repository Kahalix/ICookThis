using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace ICookThis.Utils.Email
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _config;
        private readonly IEmailBuilder _builder;

        public MailService(IConfiguration config, IEmailBuilder builder)
        {
            _config = config;
            _builder = builder;
        }

        public async Task SendAsync(string to, string subject, string bodyHtml)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config["Smtp:From"]));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = bodyHtml };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _config["Smtp:Host"],
                int.Parse(_config["Smtp:Port"]!),
                bool.Parse(_config["Smtp:UseSsl"] ?? "false")
            );
            if (!string.IsNullOrEmpty(_config["Smtp:Username"]))
                await client.AuthenticateAsync(
                    _config["Smtp:Username"]!,
                    _config["Smtp:Password"]!
                );
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}