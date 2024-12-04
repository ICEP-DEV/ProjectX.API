using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Service
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("khanyifundiswa79@gmail.com", "eijv tzsk aazr cszi")

            };

            return client.SendMailAsync(
                new MailMessage(
                    from: "khanyifundiswa79@gmail.com",
                    to: email,
                    subject,
                    message
                    ));

        }
    }
}
