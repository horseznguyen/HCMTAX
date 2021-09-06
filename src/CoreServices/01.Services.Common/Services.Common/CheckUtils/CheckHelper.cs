using System;

namespace Services.Common.CheckUtils
{
    public static class CheckHelper
    {
        public static void CheckNullOrWhiteSpace(string propertyValue, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyValue))
            {
                throw new ArgumentNullException($"{propertyName} cannot be null or empty or whitespace.", propertyName);
            }
        }
    }
}