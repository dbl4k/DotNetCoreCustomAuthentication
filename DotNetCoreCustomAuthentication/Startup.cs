using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreCustomAuthentication.Authentication;
using DotNetCoreCustomAuthentication.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetCoreCustomAuthentication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add our shiny CustomAuthentication scheme and set it as default.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CustomAuthentication.SchemeName;
                options.DefaultChallengeScheme = CustomAuthentication.SchemeName;
            })
            .AddCustomAuthentication(options =>
            {
                options.CustomSimpleAuthorizationToken = "Bearer MySuperSecretKey";
            });


            // HACK : Brute-force all MVC & API Actions to require authorization, regardless of [Authorize] attrib being present.
            // you might not want to use this, but it'll lock your controllers down safely.
            services.AddMvc(options =>
                options.Filters.Add(
                    new AuthorizeFilter(
                        new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()
                    )
                )
            );


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // enable authentication middleware
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
