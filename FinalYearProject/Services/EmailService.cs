using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FinalYearProject.Services
{
    public class EmailService
    {
        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public async Task SendEmailAsync(string Email, string Subject, string HtmlMessage)
        {
            var emailPassword = Configuration["EmailPassword"];
            const string CompanyEmailAddress = "auditcontrolsgen@gmail.com";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(CompanyEmailAddress, emailPassword),
                EnableSsl = true
            };
            await client.SendMailAsync(CompanyEmailAddress, Email, Subject, HtmlMessage);
        }
    }
}