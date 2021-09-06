using MailKit.Net.Smtp;
using MailKit.Security;
using Services.Common.Mail.Smtp;

namespace MailKit.Common
{
    public class DefaultMailKitSmtpBuilder : IMailKitSmtpBuilder
    {
        private readonly ISmtpEmailSenderConfiguration _smtpEmailSenderConfiguration;

        private readonly IMailKitConfiguration _mailKitConfiguration;

        public DefaultMailKitSmtpBuilder(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration, IMailKitConfiguration mailKitConfiguration)
        {
            _smtpEmailSenderConfiguration = smtpEmailSenderConfiguration;
            _mailKitConfiguration = mailKitConfiguration;
        }

        protected virtual void ConfigureClient(SmtpClient client)
        {
            client.Connect(_smtpEmailSenderConfiguration.Host, _smtpEmailSenderConfiguration.Port, GetSecureSocketOption());

            if (_smtpEmailSenderConfiguration.UseDefaultCredentials) return;

            client.Authenticate(_smtpEmailSenderConfiguration.UserName, _smtpEmailSenderConfiguration.Password);
        }

        protected virtual SecureSocketOptions GetSecureSocketOption()
        {
            if (_mailKitConfiguration.SecureSocketOptions.HasValue)
            {
                return _mailKitConfiguration.SecureSocketOptions.Value;
            }

            return _smtpEmailSenderConfiguration.EnableSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;
        }

        public SmtpClient Build()
        {
            var client = new SmtpClient();
            try
            {
                ConfigureClient(client);
                return client;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }
    }
}