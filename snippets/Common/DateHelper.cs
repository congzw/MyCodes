using System;

namespace CommonTrace.Common
{
    public class DateHelper
    {
        public string GetNowAsFormat(string format = "yyyyMMdd-HH:mm:ss")
        {
            var now = GetDateNow();
            return now.ToString(format);
        }

        public Func<DateTime> GetDateDefault = () => new DateTime(2000, 1, 1);
        public Func<DateTime> GetDateNow = () => DateTime.Now;
        public static DateHelper Instance = new DateHelper();
    }
}
