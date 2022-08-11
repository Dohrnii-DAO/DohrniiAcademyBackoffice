using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DohrniiBackoffice.Helpers
{
    public interface IAppUtil
    {
        string GetMsg(string alertType, string Msg);
        string GetMessage(string alertType, string Msg);
        bool IsValidEmailAddress(string value);
        bool IsValidPhoneNumber(string value);
    }
}
