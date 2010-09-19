using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace WcfHttpAuth
{
    internal static class UtcUtils
    {
        public static string UtcNowString()
        {
            return UtcString(DateTime.UtcNow);
        }

        public static string UtcString(DateTime dateTime)
        {
            return dateTime.ToString("s");
        }

        public static DateTime FromUtcString(string dateString)
        {
            return DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
        }
    }
}
