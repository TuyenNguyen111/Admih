using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace FinalProject.Services
{
    public class EmailService : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string resetpwUrl)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"]))
                {
                    smtpClient.Port = int.Parse(_configuration["EmailSettings:Port"]);
                    smtpClient.Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
                    smtpClient.EnableSsl = true;

                    var messageBody = CreateEmailBody(resetpwUrl);

                    using (var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_configuration["EmailSettings:Username"], "SmartRoomManagement"),
                        Subject = subject,
                        Body = messageBody,
                        IsBodyHtml = true,
                    })
                    {
                        mailMessage.To.Add(email);
                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }
            }
            catch (SmtpException ex)
            {
                LogError($"SMTP Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                LogError($"General Error: {ex.Message}");
                throw;
            }
        }

        private string ExtractUrlFromAnchor(string anchor)
        {
            var match = System.Text.RegularExpressions.Regex.Match(anchor, @"href=['""]?(?<url>[^'""]+)['""]?");
            return match.Success ? match.Groups["url"].Value : string.Empty;
        }

        private string CreateEmailBody(string resetpwUrl)
        {
            string extractedUrl = ExtractUrlFromAnchor(resetpwUrl);

            return $@"
                    <p>Dear Sir,</p>
                    <p>We received a request to reset your password. Please follow the steps below to complete the process:</p>
                    <ol>
                        <li>Click the link below to reset your password:</li>
                        <li><a href=""{extractedUrl}"">Reset Password Link</a></li>
                        <li>If the link does not work, copy and paste the URL into your browser's address bar.</li>
                        <li>Once on the password reset page, create a new password and confirm it.</li>
                    </ol>
                    <p>For security reasons, this link will expire in 24 hours.</p>
                    <p>Thank you,<br>SmartRoomManagement<br></p>";
        }


        private void LogError(string message)
        {
            // Ghi log vào hệ thống log của bạn
            Console.WriteLine(message); // Thay thế bằng log thực tế
        }
    }
}
