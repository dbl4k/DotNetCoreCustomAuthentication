using DotNetCoreCustomAuthentication.Authentication;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreCustomAuthentication.Extensions
{
    public static class AuthenticationBuilderExtensions
    {
        // Declaring this here will make it available to add as a strongly-typed service in Startup.
        public static AuthenticationBuilder AddCustomAuthentication(
            this AuthenticationBuilder builder,
            Action<CustomAuthentication.Options> options)
        {
            return builder
                .AddScheme<CustomAuthentication.Options, CustomAuthentication.Handler<CustomAuthentication.Options>>
                (CustomAuthentication.SchemeName, options);
        }
    }
}
