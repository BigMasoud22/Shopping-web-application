using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var from = "thisisatest22221900@outlook.com";
            var fromPassword = "@Admin22";

            var smtp = new SmtpClient
            {
                Host = "smtp-mail.outlook.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(from, fromPassword),
                EnableSsl = true,
            };

            return smtp.SendMailAsync(new MailMessage(from: from
            , to: email, subject, htmlMessage));
        }
    }
}
