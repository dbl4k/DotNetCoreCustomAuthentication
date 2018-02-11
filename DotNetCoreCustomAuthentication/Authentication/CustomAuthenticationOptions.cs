using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreCustomAuthentication.Authentication
{
    public static partial class CustomAuthentication
    {
        public class Options : AuthenticationSchemeOptions
        {
            public Options() { }

            // Any options you want to pass to the handler during instantiation, declare here.
            public string CustomSimpleAuthorizationToken { get; set; }
        }
    }
}
