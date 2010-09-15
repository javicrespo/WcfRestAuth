using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth.Wsse
{
    public interface IPasswordProvider
    {
        string RetrievePassword(string username);
    }
}
