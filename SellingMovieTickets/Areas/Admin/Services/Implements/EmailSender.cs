using System.Net.Mail;
using System.Net;
using SellingMovieTickets.Areas.Admin.Services.Interfaces;

namespace SellingMovieTickets.Areas.Admin.Services.Implements
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, // Bật bảo mật SSL
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("chodumaivesau@gmail.com", "gtesnehrxngrwslh")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("chodumaivesau@gmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true  
            };

            mailMessage.To.Add(email);
            return client.SendMailAsync(mailMessage);
        }
    }

}
