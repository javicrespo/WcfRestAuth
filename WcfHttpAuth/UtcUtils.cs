using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace WcfHttpAuth
{
    internal static class UtcUtils
    {
        private const long baseTicks = 621355968000000000;
        private const long tickResolution = 10000000;
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

        public static DateTime FromEpochString(string epochString)
        {
            return new DateTime((long.Parse(epochString) * tickResolution) + baseTicks).ToUniversalTime();
        }
    }
}
