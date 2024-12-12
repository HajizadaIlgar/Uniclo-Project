using Ab108Uniqlo.Helpers;
using Ab108Uniqlo.Services.Abstracts;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Ab108Uniqlo.Services.Implements
{
    public class EmailService : IEmailService
    {
        readonly SmtpClient _client;
        readonly MailAddress _from;
        readonly HttpContext Context;
        public EmailService(IOptions<SmtpOptions> option, IHttpContextAccessor acc)
        {
            var opt = option.Value;
            _client = new(opt.Host, opt.Port);
            _client.Credentials = new NetworkCredential(opt.Sender, opt.Password);
            _client.EnableSsl = true;
            _from = new MailAddress(opt.Sender, "Uniqlo");
            Context = acc.HttpContext;
        }

        public void SendEmailConfirmationAsync(string receiver, string token, string name)
        {
            MailAddress to = new(receiver);
            MailMessage message = new MailMessage(_from, to);
            message.Subject = "Confirm your Email";
            string url = Context.Request.Scheme + "://" + Context.Request.Host + "/Account/VeritifyEmail?token="
                 + token + "&user" + name;
            message.Body = EmailTemplates.VerifyEmail.Replace("__$name", name).Replace("__$link", url);
            message.IsBodyHtml = true;
            _client.Send(message);
        }

        public Task SendEmailConfirmationAsync()
        {
            throw new NotImplementedException();
        }
    }
}
