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
using IdentityServer4.Models;
using IdentityServer4;
using webapp.Util.Helpers;
using webapp.Services.Authorization;

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
            services.AddTransient<AuthorizationService>();
            services.AddTransient<IStorageService, S3StorageService>();
            ConfigureCaching(services, appConfig);

            services.AddMvc();
            ConfigureIdentityServer(services, appConfig);

            // https://stackoverflow.com/a/53577368/4586866
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

        private void ConfigureIdentityServer(IServiceCollection services, AppConfig appConfig)
        {
            // configure this as an identity server
            IIdentityServerBuilder identityServerBuilder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(IdentityConfigHelpers.IdentityResources)
                .AddInMemoryClients(IdentityConfigHelpers.GetClients(appConfig))
                .AddInMemoryApiResources(IdentityConfigHelpers.Apis);

            if (env.IsDevelopment())
            {
                identityServerBuilder.AddDeveloperSigningCredential();
                identityServerBuilder.AddTestUsers(TestUsers.Users);
            }
            else
            {
                // TO-DO https://codereview.stackexchange.com/a/189583 (AddSigningCredential)
            }

            // adding support for local APIs (on the same server as identity server)
            // see http://docs.identityserver.io/en/latest/topics/add_apis.html - protect with Authorize(LocalApi.PolicyName)
            services.AddLocalApiAuthentication();
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
            app.UseIdentityServer();

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
