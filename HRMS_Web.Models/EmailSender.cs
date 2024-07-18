using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HRMS_Web.Models
{
    public class EmailSender : IEmailSenderRepository
    {
        public async Task SendEmailAsync(string email, string subject, string msg, string sendermail, string senderpassword)
        {
            var mail = sendermail;
            var pw = senderpassword;

            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(mail);
                message.To.Add(email);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = msg;

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(mail, pw);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Handle exceptions (logging, rethrowing, etc.)
                throw new Exception("Email sending failed.", ex);
            }
        }
    }
}