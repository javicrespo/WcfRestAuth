using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth
{
    public interface IPasswordProvider
    {
        string RetrievePassword(string username);
    }
}
