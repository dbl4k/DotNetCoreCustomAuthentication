# DotNetCore CustomAuthentication

A primer on how to use the standard [Authorize] filter attribute and dynamically generate/read User Claims 
with non-standard authentication methods.

dot net is great at giving you almost out-of-the-box identity management developer experience. 
The most obvious choice is the **Identity** framework with all it's baked in shortcuts to integrate 
with entity framework persistence etc..

Also there's some pre-rolled external auth providers for a wide range of well known services and styles: 
Facebook, Google to the more generalized OAuth 2.

This is all very great, but what if you're in a position where you need to authenticate against a third party who doesn't follow any of these standards? What if you don't have a SQL Server available? What if you need to do something so out of the norm but you need to implement in a standard, declarative fashion rather than going full-hack on it.

Good news, it's totally possible! 

But there's not much in the way of documentation, writeup or usage example in the wild right now.

Onward!

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

This is a demo on setting up one method, or Scheme. I've consistently called it *CustomAuthentication...*. I do recommend using a relevant naming convention. 

Also, for neatness, don't try to cram different fundamental auth methods into one extension, seperate them out. i.e. Cookie Authentication should probably not be combined with Header authentication. Just create a suitable Handler and Options for each and add to the Extensions, then you can chain them up independently!

- dbl4k, 2018-02-11
