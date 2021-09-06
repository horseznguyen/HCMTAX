using System;
using System.ComponentModel;

namespace Services.Common.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T enumValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                return null;
            }

            var description = enumValue.ToString();

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString() ?? throw new InvalidOperationException());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);

                if (attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }
    }
}