using System;

namespace Services.Common.DateTimeUtils
{
    public static class DateTimeExtensions
    {
        public static double GetTimeStamp(this DateTime dateTime, bool includedTimeValue = false)
        {
            var date = includedTimeValue ? dateTime : dateTime.Date;

            return Math.Round(date.Subtract(new DateTime(1970, 01, 01)).TotalSeconds, 0);
        }
    }
}