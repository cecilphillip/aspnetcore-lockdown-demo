
using System.Threading.Tasks;

namespace DayCare.Web
{
    using System;
    using Models;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Security.Claims;
    using Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Requirements;

    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public ILoggerFactory LogFactory { get; set; }
        public IHostingEnvironment HostingEnv { get; set; }

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            HostingEnv = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            LogFactory = loggerFactory;
        }



        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.GuardianPolicyName, policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Guardian"));
                //options.AddPolicy("Guardian", policyBuilder => policyBuilder.RequireRole("Guardian"));

                options.AddPolicy(Constants.StaffPolicyName, policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Staff", "Admin"));
                //options.AddPolicy("Staff", policyBuilder => policyBuilder.RequireRole("Staff"));
            });

            services.AddTransient<IAuthorizationHandler, AssignedStaffAddNoteHandler>();
            services.AddTransient<IAuthorizationHandler, StaffAdminAddNoteHandler>();

            // Wiring up InMemory database only for testing
            services.AddDbContext<DayCareContext>(options =>
            {
                options.UseInMemoryDatabase();
            }, ServiceLifetime.Singleton);

            services.AddAntiforgery();

            services.AddMvc(opts =>
            {
                var defaultPolicy = new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .Build();

                opts.Filters.Add(new AuthorizeFilter(defaultPolicy));
            });

            services.AddTransient<IDayCareService, DayCareService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            if (HostingEnv.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async ctx =>
                    {
                        var errorFeature = ctx.Features.Get<IExceptionHandlerFeature>();
                        var error = errorFeature.Error; // Do what you want with this error

                        var errorLogger = LogFactory.CreateLogger<Startup>();
                        errorLogger.LogError(911, error.Message, error);

                        var responseData = new
                        {
                            Message = "Sorry, something when wrong. Please try again later",
                            DateTime = DateTimeOffset.Now
                        };

                        await ctx.Response.WriteAsync(JsonConvert.SerializeObject(responseData));
                    });
                });
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

            app.UseClaimsTransformation(ctx =>
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, "Attendee")
                };

                var id = new ClaimsIdentity(claims, "COTB");
                ctx.Principal.AddIdentity(id);

                return Task.FromResult(ctx.Principal);
            });

            app.UseStatusCodePagesWithReExecute("/error/{0}");

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
