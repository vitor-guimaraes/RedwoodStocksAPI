using System.Net;
using System.Net.Mail;

namespace RedwoodStocksAPI.Domain.Services
{
    public class SendEmailService
    {
        public static void SendEmail(string message, string timestamp)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath = Path.Combine(desktopPath, "Output");
            string filePath = Path.Combine(folderPath, $"STOCKS_{timestamp}.csv");
            string logsFolderPath = Path.Combine(desktopPath, "logs");
            string logFilePath = Path.Combine(logsFolderPath, "error_log.txt");

            // Setup mail message
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("Papercut@papercut.com");
            mail.To.Add("Papercut@user.com");
            mail.Subject = "Stocks Information";
            mail.Body = message;

            // Setup SMTP client to use localhost
            SmtpClient smtpClient = new SmtpClient("localhost", 25);
            smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                // Send the email
                if (File.Exists(filePath))
                {
                    var attachment = new System.Net.Mail.Attachment($"{folderPath}/STOCKS_{timestamp}.csv");
                    mail.Attachments.Add(attachment);
                }
                else
                {
                    var attachment = new System.Net.Mail.Attachment($"{logFilePath}");
                    mail.Attachments.Add(attachment);
                }
                smtpClient.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
