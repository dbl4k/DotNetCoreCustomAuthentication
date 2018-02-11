using DotNetCoreCustomAuthentication.Generators;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DotNetCoreCustomAuthentication.Authentication
{
    public static partial class CustomAuthentication
    {

        public class Handler<TOptions> : AuthenticationHandler<TOptions> where TOptions : Options, new()
        {
            protected static class Messages
            {
                public const string EInvalidHeader = "Authorization header and cookie not present.";
                public const string EInvalidCredentials = "Invalid credentials supplied.";
            }

            public Handler(IOptionsMonitor<TOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
                : base(options, logger, encoder, clock) { }

            protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                // This is where you plumb in your actual custom authentication logic.
                // you have access to the full Request and Response so you can spin it however you want:

                // some ideas: 

                // To access headers you can use: Request.Headers.TryGetValue(...)
                // To access cookies you can use: Request.Cookies.TryGetValue(...)
                // You also have access to request bodies, querystrings, you name it.

                // To set cookies you can Response.Cookies.Append(...)
                // recommend not mixing cookie and header auth in the same handler (tight coupling).

                // To grant access you can return AuthenticateResult.Success(ticket))
                // To deny access you can return AuthenticateResult.Fail(message)

                // Super-simple example below will validate a header Authorization (key is in startup).

                Request.Headers.TryGetValue(HeaderNames.Authorization, out var authheaders);

                if (authheaders.Count == 0)
                {
                    return await Task.Run(() =>
                        AuthenticateResult.Fail(Messages.EInvalidHeader)
                    );
                }

                // The auth key from Authorization header check against the configured one
                if (authheaders != Options.CustomSimpleAuthorizationToken)
                {
                    return await Task.Run(() =>
                        AuthenticateResult.Fail(Messages.EInvalidCredentials)
                    );
                }

                // If we've got this far, we're considered to be authenticated.

                // Generate an on-the-fly claims based identity for use in controllers, actions, views et..
                // bear in mind we don't have a user-store in this context it's only used during the scope.
                var identity = new ClaimsIdentity(SchemeName);

                // set some randomly generated claim values to prove we can read them in the controller.
                identity.AddClaim(new Claim("FirstName", RandomName.GetFirstName()));
                identity.AddClaim(new Claim("LastName", RandomName.GetLastName()));

                List<ClaimsIdentity> identities =
                    new List<ClaimsIdentity> { identity };

                AuthenticationTicket ticket =
                    new AuthenticationTicket(new ClaimsPrincipal(identities), SchemeName);

                // return a success state.
                return await Task.Run(() =>
                        AuthenticateResult.Success(ticket)
                    );
            }
        }
    }
}
