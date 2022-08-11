using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DohrniiBackoffice.Helpers
{
    public class AuthConstants
    {
        public const string AuthSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;
        public const string AdminRole = "Admin";
        public const string UserRole = "User";
        public const string EmailRegex = @"\A(?:[a-z0-9]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
        public const string PhoneRegex = "\\A[0-9]{3}-[0-9]{3}-[0-9]{4}\\z";
        public const string DefaultProfile = "/imagefiles/defaultuser.png";
        public const string HtmlWelcomeTemplate =
            @"
<html>
<head>
    <style>
        
        p {
            font-size: 14px;
            font-weight: 400;
            line-height: 141%;
            letter-spacing: -0.01em;
            color: #070707;
        }
        span {
            color: #7892FD;
            font-size: 14px;
            font-weight: 400;
            line-height: 141%;
            letter-spacing: -0.01em;
        }
    </style>
</head>
<body>
    <p>Hi, <br/> We're excited to have you get started at the Dohrnii Academy Platform. First, you need to verify your account and here is your verification code.</p>

    <p>
        <b>{{code}}</b>
    </p>


    <p>
        If you have any questions, just reply to this email—we're always happy to help out.

    </p>
    <p>
        Cheers,<br>Dohrnii Team
    </p>
</body>
</html>
";
    }
}
