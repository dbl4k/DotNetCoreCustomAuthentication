# DotNetCore CustomAuthentication

A brief, informal primer on how to use the standard **Authorize** filter attribute, with all the goodness of on-the-fly **User** & **UserClaims** objects, thru non-standard authentication methods.

.net is great at giving you an out-of-the-box identity management developer experience, removing the repeated drawl and oft improperly implemented trudge of writing your own authentication systems for every web app you produce. 
The most obvious choice in any [greenfield](https://en.wikipedia.org/wiki/Greenfield_project) is the **Identity** framework with all it's shortcuts to integrate 
with entity framework, including pre-rolled external provider logic for a wide range of [well known services and authentication schemes](https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/identity-2x) and even some [additional community supported providers](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/tree/dev/src).

This is all great, but what if you're in a position where you need to authenticate using a third party service that *doesn't* follow any of these standard mechanisms? 

What if you have no persisted user stores: Let's say the user details are coming from an external provider during authentication. You have no need or reason to persist or maintain your own local user store.

## What if you need to do something a bit leftfield, but you still want to integrate it as cleanly as possible via Identity.

You're probably here because you've been running in circles. Admittedly, there's not a good amount of information in the wild on implementing bespoke authentication via .net core 2.0 right now.

Good news: It's completely possible by implementing your own **Extension**, **Options** and **Handler** classes - not as scary as it sounds. Stick with me.

A basic structure you'd use to achieve this could be:

* Authentication 
  * [CustomAuthentication](https://github.com/dbl4k/DotNetCoreCustomAuthentication/blob/master/DotNetCoreCustomAuthentication/Authentication/CustomAuthentication.cs)
  * [CustomAuthenticationOptions](https://github.com/dbl4k/DotNetCoreCustomAuthentication/blob/master/DotNetCoreCustomAuthentication/Authentication/CustomAuthenticationOptions.cs)
  * [CustomAuthenticationHandler](https://github.com/dbl4k/DotNetCoreCustomAuthentication/blob/master/DotNetCoreCustomAuthentication/Authentication/CustomAuthenticationHandler.cs)
* Extensions
  * [AuthenticationBuilderExtensions](https://github.com/dbl4k/DotNetCoreCustomAuthentication/blob/master/DotNetCoreCustomAuthentication/Extensions/AuthenticationBuilderExtensions.cs)
* [Startup](https://github.com/dbl4k/DotNetCoreCustomAuthentication/blob/master/DotNetCoreCustomAuthentication/Startup.cs)
* Controllers
  * [ValuesController.cs](https://github.com/dbl4k/DotNetCoreCustomAuthentication/blob/master/DotNetCoreCustomAuthentication/Controllers/ValuesController.cs)
## The Breakdown

Folder          | Class                           | Purpose
---             | ---                             | ---
Authentication\ | CustomAuthentication            | Defines the partial class, contains only a constant with the scheme name.
Authentication\ | CustomAuthenticationOptions     | A custom options object that you can populate with neccessary values and pass to the handler during instantiation
Authentication\ | CustomAuthenticationHandler     | This is where you can plumb in your custom authentication logic, you have full acccess to the **Request**, **Response** including headers, cookies, urlparams, body etc.. The **Handler** method must return a relevant **AuthenticateResult**.
Extensions\     | AuthenticationBuilderExtensions | Binds the **Options** and **Handler** together into an AddCustomAuthentication **Extension**, which will be used in Startup (see fig i. below).
\               | Startup                         | Utilises the **AddCustomAuthentication** extension and passes it a populated **Options** instance when needed.
Controllers\    | ValuesController                | A brief example of extracting Claims values from the User object we created during authentication.

### fig i. Chaining the fluent **AddCustomAuthentication** extension
```csharp
public void ConfigureServices(IServiceCollection services)
{
  // <code omitted>
 
  services.AddAuthentication(options =>
  {
      options.DefaultAuthenticateScheme = CustomAuthentication.SchemeName;
      options.DefaultChallengeScheme = CustomAuthentication.SchemeName;
  })
  .AddCustomAuthentication(options =>
  {
      options.CustomSimpleAuthorizationToken = "Bearer MySuperSecretKey";
  });

  // <code omitted>
}
```

## Usage

GET http://localhost:63276/api/values

Using **Request** Headers:

| Key           | Value                   |
| ---           | ---                     |
| Authorization | Bearer MySuperSecretKey |

Should provide you with a **Response** containing the randomly generated name set as UserClaim *FirstName* and *LastName* during the authentication process.

```json
[
    "Tommy",
    "Jones"
]
```

In reality, the names wouldn't be randomly generated, they could be provided by an external source that you're authenticating with.

You could add other claim key/value pairs, FirstName and LastName are just some obvious examples.

## Recommendations: 

This is a demo on setting up one method, or **Authentication Scheme** as it's referred to. 

In the real world you wouldn't call these classes **CustomAuthentication...**, you'd probably choose soemthing more relevant to the actual authentication mechanism you're using.

Also, for neatness, don't try to cram different fundamental auth methods into one extension, seperate them out. i.e. Cookie Authentication should probably not be combined with Header Authentication. Just create a suitable Handler and Options for each and add to the Extension builder, then you can chain them up in Startup independently!


One last thing, might sound obvious, but don't hardcode credentials and sensitive information into your code. Bad! I've done it here for brevity's sake. You'll want to feed these into the Options via your IConfiguration implementation, this is a whole other discipline I won't go into here ([best practises](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio))

Happy breadmaking!

- dbl4k, 2018-02-11
