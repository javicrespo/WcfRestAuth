using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace WcfRestAuth.Basic
{
    internal class BasicToken
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
