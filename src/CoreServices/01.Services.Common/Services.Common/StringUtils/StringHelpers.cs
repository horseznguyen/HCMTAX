using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Services.Common.StringUtils
{
    public static class StringHelpers
    {
        public static string GenerateRandom(int length, params char[] chars)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"{length} cannot be less than zero.");
            }

            if (chars?.Any() != true)
            {
                throw new ArgumentOutOfRangeException(nameof(chars), $"{nameof(chars)} cannot be empty.");
            }

            chars = chars.Distinct().ToArray();

            const int maxLength = 256;

            if (maxLength < chars.Length)
            {
                throw new ArgumentException($"{nameof(chars)} may contain more than {maxLength} chars.", nameof(chars));
            }

            var outOfRangeStart = maxLength - (maxLength % chars.Length);

            using (var rng = RandomNumberGenerator.Create())
            {
                var sb = new StringBuilder();

                var buffer = new byte[128];

                while (sb.Length < length)
                {
                    rng.GetBytes(buffer);

                    for (var i = 0; i < buffer.Length && sb.Length < length; ++i)
                    {
                        if (outOfRangeStart <= buffer[i])
                        {
                            continue;
                        }

                        sb.Append(chars[buffer[i] % chars.Length]);
                    }
                }

                return sb.ToString();
            }
        }
    }
}