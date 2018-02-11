# DotNetCore CustomAuthentication

A brief, informal primer on how to use the standard **Authorize** filter attribute, with all the goodness of on-the-fly **User** & **UserClaims** objects, thru non-standard authentication methods.

dot net is great at giving you an out-of-the-box identity management developer experience. 
The most obvious choice in any greenfield is the **Identity** framework with all it's shortcuts to integrate 
with entity framework, including pre-rolled external provider logic for a wide range of well known services and authentication providers.

This is all great, but what if you're in a position where you need to authenticate using a third party service that *doesn't* follow any of these mechanisms? 

What if you don't have a SQL Server available? 

What if you need to do something a bit leftfield, but you still want to integrate it as cleanly as possible.

You're probably here because you've been running in circles. Admittedly, there's not a good amount of information in the wild on implemnting bespoke authentication via .net core 2.0 right now.

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
\               | Startup                         | Utilises the **AddCustomAuthentication** extension and passes it the **Options** when needed.
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

## Recommendations: 

This is a demo on setting up one method, or **Authentication Scheme** as it's referred to. 

In the real world you wouldn't call these classes **CustomAuthentication...**, you'd probably choose soemthing more relevant to the actual authentication mechanism you're using.

Also, for neatness, don't try to cram different fundamental auth methods into one extension, seperate them out. i.e. Cookie Authentication should probably not be combined with Header Authentication. Just create a suitable Handler and Options for each and add to the Extension builder, then you can chain them up independently!

Happy breadmaking!

- dbl4k, 2018-02-11
