using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth.Wsse
{
    /// <summary>
    /// Timestamp range validator
    /// </summary>
    public interface ITimestampRangeValidator
    {
        /// <summary>
        /// Validates the timestamp.
        /// </summary>
        /// <param name="messageUtcTimestamp">The message UTC timestamp.</param>
        /// <returns>If the timestamp is withing the predefined limits</returns>
        bool ValidateTimestamp(DateTime messageUtcTimestamp);
    }
}
