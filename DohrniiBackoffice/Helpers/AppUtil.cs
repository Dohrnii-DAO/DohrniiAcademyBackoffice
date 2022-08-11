using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DohrniiBackoffice.Helpers
{
    public class AppUtil : IAppUtil
    {
        public string GetMsg(string alertType, string Msg)
        {
            return $"<div class=\"alert alert-{alertType} alert-dismissible fade show\" role=\"alert\">{Msg}<button type=\"button\" class=\"btn-close btn-sm\" data-bs-dismiss=\"alert\" aria-label=\"Close\"></button></div>";
        }

        public string GetMessage(string alertType, string Msg)
        {
            return $"<div class=\"alert alert-{alertType} alert-dismissible\" role=\"alert\"><button type=\"button\" data-dismiss=\"alert\" class=\"close\" aria-label=\"Close\"><span aria-hidden=\"true\">×</span></button>{ Msg}</div>";
        }

        public bool IsValidEmailAddress(string value)
        {
            try
            {
                bool isEmail = Regex.IsMatch(value.Trim(), AuthConstants.EmailRegex);
                return isEmail;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsValidPhoneNumber(string value)
        {
            try
            {
                bool isPhone = Regex.IsMatch(value.Trim(), AuthConstants.PhoneRegex);
                return isPhone;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    public class ResponseViewModel
    {
        public bool IsSuccessful { get; set; } = false;
        public string Msg { get; set; }
    }


}

public enum alert
{
    success,
    danger,
    warning
}
