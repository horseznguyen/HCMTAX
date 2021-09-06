using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace EventBus.CAP
{
    public class BasicAuthAuthorizationUser
    {
        /// <summary>
        /// Represents user's name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// SHA1 hashed password.
        /// </summary>
        public byte[] Password { get; set; }

        /// <summary>
        /// Setter to update password as plain text.
        /// </summary>
        public string SetPassword
        {
            set
            {
                using (var cryptoProvider = SHA1.Create())
                {
                    Password = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(value));
                }
            }
        }

        public bool Validate(string username, string password, bool loginCaseSensitive)
        {
            if (string.IsNullOrWhiteSpace(username) == true)
            {
                throw new ArgumentNullException("login");
            }

            if (string.IsNullOrWhiteSpace(password) == true)
            {
                throw new ArgumentNullException("password");
            }

            if (username.Equals(username, loginCaseSensitive ? StringComparison.CurrentCulture : StringComparison.OrdinalIgnoreCase) == true)
            {
                using (var cryptoProvider = SHA1.Create())
                {
                    byte[] passwordHash = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(password));

                    return StructuralComparisons.StructuralEqualityComparer.Equals(passwordHash, Password);
                }
            }
            else
            {
                return false;
            }
        }
    }
}