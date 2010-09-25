using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth.Wsse
{
    /// <summary>
    /// Password provider
    /// </summary>
    public interface IPasswordProvider
    {
        /// <summary>
        /// Retrieves the password by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>Password</returns>
        string RetrievePassword(string username);
    }
}
