using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth
{
    internal static class NonceGenerator
    {
        public static String Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
