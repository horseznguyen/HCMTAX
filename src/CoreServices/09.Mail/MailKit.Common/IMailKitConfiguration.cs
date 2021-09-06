using MailKit.Security;

namespace MailKit.Common
{
    public interface IMailKitConfiguration
    {
        SecureSocketOptions? SecureSocketOptions { get; set; }
    }
}