
using MailKit.Net.Smtp;
using MimeKit;

namespace TodoAPI.Services
{
    public class EmailSender
    {

        private readonly IConfiguration _config;
        public string SenderEmail { get; } = "cratecrarity@gmail.com";
        public string SenderName { get; } = "Prioritia";
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendEmail(string name, string email, string subject, string message)
        {
            var messageToSend = new MimeMessage();
            messageToSend.From.Add(new MailboxAddress(SenderName, SenderEmail));
            messageToSend.To.Add(new MailboxAddress(name, email));
            messageToSend.Subject = subject;

            //send the body as HTML
            messageToSend.Body = new TextPart("html")
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);

                //get get Gmail password and authenticate
                string password = _config.GetValue<string>("Authentication:Gmail:Password") ?? throw new KeyNotFoundException("Email authentication password not found.");

                client.Authenticate(SenderEmail, password);

                client.Send(messageToSend);
                client.Disconnect(true);

            }
        }
    }
}
