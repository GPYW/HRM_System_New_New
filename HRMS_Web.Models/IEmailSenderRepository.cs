namespace HRMS_Web.Models
{
    public interface IEmailSenderRepository
    {
        Task SendEmailAsync(string email, string subject, string msg, string sendermail, string senderpassword);

    }
}