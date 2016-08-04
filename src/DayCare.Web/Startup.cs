using DayCare.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DayCare.Web
{
    using Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Authorization;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            // Wiring up InMemory database only for testing
            services.AddDbContext<DayCareContext>(options =>
            {
                options.UseInMemoryDatabase();
            }, ServiceLifetime.Singleton);

            services.AddMvc(opts =>
            {
                var defaultPolicy = new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .Build();

                opts.Filters.Add(new AuthorizeFilter(defaultPolicy));
            });

            services.AddTransient<ISecurityService, SecurityService>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = Constants.AppCookieMiddlewareScheme,
                CookieHttpOnly = true,
                CookieSecure = CookieSecurePolicy.SameAsRequest,

                LoginPath = new PathString(Constants.LoginPath),
                AccessDeniedPath = new PathString(Constants.DeniedPath), //TODO: Addd a denied path

                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseMvcWithDefaultRoute();
        }
    }
}
