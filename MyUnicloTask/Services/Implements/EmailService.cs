using Ab108Uniqlo.Helpers;
using Ab108Uniqlo.Services.Abstracts;
using Microsoft.Extensions.Options;

namespace Ab108Uniqlo.Services.Implements
{
    public class EmailService : IEmailService
    {
        readonly SmtpOptions _smtpOptions;
        public EmailService(IOptions<SmtpOptions> option)
        {
            _smtpOptions = option.Value;
        }
        public Task SendAsync()
        {
            var a = _smtpOptions;
            return Task.CompletedTask;
        }
    }
}
