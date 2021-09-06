using System;

namespace Services.Common.Utilities
{
    public static class DateTimeHelpers
    {
        public static DateTime TimeStampToDate(double timeStamp)
        {
            timeStamp = timeStamp < 0 ? 0 : timeStamp;

            var date = TimeStampToDateTime(timeStamp);

            return date.Date;
        }

        public static DateTime TimeStampToDateTime(double timeStamp)
        {
            timeStamp = timeStamp < 0 ? 0 : timeStamp;

            DateTimeOffset offset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timeStamp));

            return offset.DateTime;
        }

        public static DateTime Now(DateTimeKind kind) => kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;
    }
}