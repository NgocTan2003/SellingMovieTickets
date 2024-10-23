using System.Net.Mail;
using System.Net;

namespace SellingMovieTickets.Areas.Admin.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("chodumaivesau@gmail.com", "gtesnehrxngrwslh")
            };

            return client.SendMailAsync(
                new MailMessage(from: "chodumaivesau@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
