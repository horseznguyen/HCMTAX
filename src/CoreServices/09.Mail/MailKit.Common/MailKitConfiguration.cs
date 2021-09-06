using MailKit.Security;

namespace MailKit.Common
{
    public class MailKitConfiguration : IMailKitConfiguration
    {
        public SecureSocketOptions? SecureSocketOptions { get; set; }
    }
}