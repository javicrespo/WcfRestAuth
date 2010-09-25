using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth.Wsse
{
    /// <summary>
    /// Default Timestamp range Validator
    /// </summary>
    public class DefaultTimestampRangeValidator:ITimestampRangeValidator
    {
        private TimeSpan range;

        public DefaultTimestampRangeValidator()
            :this(new TimeSpan(0, 10, 0))
        {
        }

        public DefaultTimestampRangeValidator(TimeSpan range)
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
