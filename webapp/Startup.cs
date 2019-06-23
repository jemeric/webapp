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
            ISchema schema = GraphQLService.LoadSchema($"{env.ContentRootPath}/Assets/GraphQL/schema.graphql");

            services.AddMvc();

            // configure this as an identity server
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                // http://docs.identityserver.io/en/latest/topics/resources.html
                // define the resources to protect (identity resource has unique name and you can assign arbitrary claim types to it)
                // will be included in the identity toekn for the user (client will use the scope parameter to request access to identity resource)
                // OpenID Connect spec - minimum requirement is you provide support for emitting a unique ID for your users (the subject id)
                // this is done by exposing the standard identity resource (openid)
                .AddInMemoryIdentityResources(new List<IdentityResource>()
                {
                    new IdentityResources.OpenId() // TO-DO move to separate config file
                })
                .AddInMemoryClients(new List<Client>()
                {
                    // backend client
                    new Client
                    {
                        ClientId = "client",
                        ClientName = "Backend Client",

                        // no interactive user, use the clientid/secret for authentication
                        // (server-to-server communication - tokens always requested on behalf of client, not user)
                        AllowedGrantTypes = GrantTypes.ClientCredentials,

                        // secret for authentication
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },

                        // scopes the client has access to
                        AllowedScopes = { IdentityServerConstants.LocalApi.ScopeName }

                    },

                    // JavaScript client
                    new Client
                    {
                        ClientId = "spa",
                        ClientName = "JavaScript Client",
                        // for Javascript-based applications use Authorization Code with PKCE instead of implicit
                        // Code (Authorization Code) = provides a way to retrieve tokens on a back-channel as opposed to browser front-channel
                        AllowedGrantTypes = GrantTypes.Code,
                        // "Proof Key for Code Exchange" - way to make OAuth 2.0 and OpenID Connect operations using an authorization code more secure
                        // applies to authorization/token requests whenever code grant type is involved
                        RequirePkce = true,
                        RequireClientSecret = false,

                        RedirectUris = {"https://localhost:5003/callback.html"},
                        PostLogoutRedirectUris = {"https://localhost:5003/index.html"},
                        // TODO - always use current address (add additional for local dev CORS)
                        AllowedCorsOrigins = {"https://localhost:5003"},

                        // scopes the client has access to
                        AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.LocalApi.ScopeName }
                    }

                    // TODO - Google Auth?
                })
                // http://docs.identityserver.io/en/latest/topics/resources.html#defining-api-resources
                // to allow clients to request access tokens for your APIs you need to define API resources
                // to get access tokesn for APIs you need to register them as a scope
                .AddInMemoryApiResources(new List<ApiResource>()
                {
                    // this assigns scope for using the AddLocalApiAuthentication helper
                    new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
                });

            // adding support for local APIs (on the same server as identity server)
            services.AddLocalApiAuthentication();

            // https://stackoverflow.com/a/53577368/4586866
            services.AddHttpContextAccessor();

            // setup dependencies for injection here
            services.AddSingleton<NPMManagerService>();
            services.AddScoped<SettingsService>();
            //services.AddSingleton<GraphQLService>();
            services.AddGraphQL(schema);
            //services.AddSingleton<QueryResolver>();
            // TODO - this could be broken into multiple initializers if it becomes more complex
            // may need to be run in parallel - https://github.com/thomaslevesque/AspNetCore.AsyncInitialization/issues/8 
            services.AddAsyncInitializer<WebAppInitializer>();
            // add distributed cache
            if(configuration.GetValue<bool>("IsDistributed"))
            {
                // TODO - use Redis https://dotnetcoretutorials.com/2017/01/06/using-redis-cache-net-core/
            }
            else {
                services.AddDistributedMemoryCache();
                services.AddScoped<IAssetsService, InMemoryAssetsService>();
            }
            // add local cache
            services.AddMemoryCache();
            services.AddSingleton(BindConfig<S3Configuration>("S3"));
            services.AddSingleton(BindConfig<AssetsConfiguration>("Assets"));
            services.AddTransient<IStorageService, S3StorageService>();

            //services.AddSingleton<NPMManagerService>((ctx) =>
            //{
            //    // example using provider (implementationFactory)
            //    IHostingEnvironment env = ctx.GetService<IHostingEnvironment>();
            //    return await NPMManagerService.BuildNPMManagerService(env);
            //});

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
