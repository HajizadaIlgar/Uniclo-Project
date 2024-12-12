namespace Ab108Uniqlo.Services.Abstracts
{
    public interface IEmailService
    {
        void SendEmailConfirmationAsync(string receiver, string name, string token);
    }
}
