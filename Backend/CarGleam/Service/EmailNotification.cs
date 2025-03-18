using System.Net; // for networkcredential
using System.Net.Mail; // for smtpclient and mailmessage

namespace CarGleam.Services
{
    public class EmailNotificationService
    {
        private readonly IConfiguration _configuration; // used for reading appsettings.json
        public EmailNotificationService(IConfiguration configuration)   // constructor of class
        {
            _configuration = configuration;  // dependency injection
        }
        public async Task SendEmailAsync(string toEmail, string subject, string message) // async method to send email
        {
            //Console.WriteLine("Starting to send email..."); -- for debugging

            var smtpClient = new SmtpClient() // smtpclient is object
            {
                Host = _configuration["Smtp:Host"],
                Port = int.Parse(_configuration["Smtp:Port"]), // convert string to int
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]), 
                EnableSsl = true, 
            };

            var mailMessage = new MailMessage // mailmessage is object
            {
                From = new MailAddress(_configuration["Smtp:FromEmail"]), 
                Subject = subject,
                Body = message,
                IsBodyHtml = true, // properties of object mailmsg
            };
            mailMessage.To.Add(toEmail); // add email to mailmessage

            //Console.WriteLine($"Sending email to {toEmail} with subject '{subject}'");

            await smtpClient.SendMailAsync(mailMessage); // send email

            //Console.WriteLine("Email sent successfully.");
        }
    }
}