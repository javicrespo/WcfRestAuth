using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth.Basic
{
    /// <summary>
    /// Authentication service
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Validates the specified username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        bool Validate(string username, string password);
    }
}
