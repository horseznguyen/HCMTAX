using MailKit.Net.Smtp;

namespace MailKit.Common
{
    public interface IMailKitSmtpBuilder
    {
        SmtpClient Build();
    }
}