using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth
{
    public class TimestampRangeValidator:ITimestampRangeValidator
    {
        private TimeSpan range;

        public TimestampRangeValidator()
            :this(new TimeSpan(0, 10, 0))
        {
        }

        public TimestampRangeValidator(TimeSpan range)
        {
            this.range = range;
        }

        public bool ValidateTimestamp(DateTime messageUtcTimestamp)
        {
            var now = DateTime.UtcNow;
            return now.Add(range) > messageUtcTimestamp && now.Add(-range) < messageUtcTimestamp;
        }
    }
}
