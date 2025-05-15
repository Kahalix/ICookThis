using System.Threading.Tasks;

namespace ICookThis.Utils.Email
{
    public interface IMailService
    {
        Task SendAsync(string to, string subject, string bodyHtml);
    }
}