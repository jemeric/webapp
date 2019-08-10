using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using webapp.Services;
using webapp.Services.Initialization;
using webapp.Models;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Playground;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using webapp.Services.Assets;
using webapp.Util.Dto.Configuration;
using webapp.Services.Storage;
using Microsoft.AspNetCore.HttpOverrides;
using webapp.Util;
using webapp.Services.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace webapp
{
    public class Startup
    {
        private readonly IHostingEnvironment env;
        private readonly IConfiguration configuration;

        // https://stackoverflow.com/a/38792310/4586866
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            this.env = env;
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // see https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.2
            //services.AddMemoryCache();
            // TODO: it should be possible to load the schema without blocking the services (create + link issue)
            AppConfig appConfig = BindConfig<AppConfig>("App");
            services.AddSingleton<AppConfig>(appConfig);

            // setup dependencies for injection here
            services.AddSingleton<NPMManagerService>();
            services.AddScoped<SettingsService>();
            services.AddScoped<AuthorizationService>();
            services.AddTransient<IStorageService, S3StorageService>();
            ConfigureCaching(services, appConfig);

            services.AddMvc();
            ConfigureAuthentication(services, appConfig);
            
            // using context accessor to get current host https://stackoverflow.com/a/53577368/4586866
            services.AddHttpContextAccessor();

            ConfiguratGraphQL(services);
            // TODO - this could be broken into multiple initializers if it becomes more complex
            // may need to be run in parallel - https://github.com/thomaslevesque/AspNetCore.AsyncInitialization/issues/8 
            services.AddAsyncInitializer<WebAppInitializer>();
        }

        private static void ConfigureCaching(IServiceCollection services, AppConfig appConfig)
        {
            // add distributed cache
            if (appConfig.IsDistributed)
            {
                // TODO - use Redis https://dotnetcoretutorials.com/2017/01/06/using-redis-cache-net-core/
            }
            else
            {
                services.AddDistributedMemoryCache();
                services.AddScoped<IAssetsService, InMemoryAssetsService>();
            }
            // add local cache
            services.AddMemoryCache();
        }

        private void ConfiguratGraphQL(IServiceCollection services)
        {
            ISchema schema = GraphQLService.LoadSchema($"{env.ContentRootPath}/Assets/GraphQL/schema.graphql");
            services.AddGraphQL(schema);
        }

        private void ConfigureAuthentication(IServiceCollection services, AppConfig appConfig)
        {
            // See https://www.c-sharpcorner.com/article/authentication-and-authorization-in-asp-net-core-mvc-using-cookie/
            // use same-server cookies to secure SPA https://leastprivilege.com/2019/01/18/an-alternative-way-to-secure-spas-with-asp-net-core-openid-connect-oauth-2-0-and-proxykit/
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            // create authentication middleware service (AuthenticationScheme sets the default authentication scheme for the app - useful when multiple instances of cookie auth)
            // the constant below sets it to "Cookies" but you can provide any string value that distinguishes the scheme
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
        }

        private T BindConfig<T>(string bindToConfigName) where T : new()
        {
            var config = new T();
            configuration.Bind(bindToConfigName, config);
            return config;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles(); // allow reference to static files in wwwroot

            // invokes the authentication middleware that sets the HttpContext.User property
            app.UseCookiePolicy();
            app.UseAuthentication();

            // server files outside of web root
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    System.IO.Path.Combine(env.ContentRootPath, AppConstants.webRoot)),
                RequestPath = "/dist"
            });

            //// use forwarded headers (https://stackoverflow.com/questions/39113100/getting-127-0-0-1-when-using-httpcontext-features-getihttpconnectionfeature)
            //var forwardOpts = new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All };
            //forwardOpts.KnownNetworks.Clear();
            //forwardOpts.KnownProxies.Clear();
            //app.UseForwardedHeaders(forwardOpts);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // intercepts requests that would match files built by webpack and dynamically builds those files on demand
                // browser is guaranteed to receive up-to-date build output
                // instance of webpack stays active and has partial compilation states pre-cached in memory
                // requires aspnet-webpack
                // change project path so webpack middleware knows where to look
                app.UseWebpackDevMiddleware(new Microsoft.AspNetCore.SpaServices.Webpack.WebpackDevMiddlewareOptions
                {
                    ProjectPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "ClientApp"),
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }

            app.UseGraphQL("/api/graphql");
            app.UsePlayground(new PlaygroundOptions()
            {
                Path = "/playground",
                QueryPath = "/api/graphql"
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

    }
}
