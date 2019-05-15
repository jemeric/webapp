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
using GraphiQl;
using GraphQL.Types;

namespace webapp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // see https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.2
            //services.AddMemoryCache();

            services.AddMvc();

            // setup dependencies for injection here
            services.AddSingleton<NPMManagerService>();
            services.AddSingleton<GraphQLService>();
            // TODO - this could be broken into multiple initializers if it becomes more complex
            // may need to be run in parallel - https://github.com/thomaslevesque/AspNetCore.AsyncInitialization/issues/8 
            services.AddAsyncInitializer<WebAppInitializer>();
            //services.AddSingleton<NPMManagerService>((ctx) =>
            //{
            //    // example using provider (implementationFactory)
            //    IHostingEnvironment env = ctx.GetService<IHostingEnvironment>();
            //    return await NPMManagerService.BuildNPMManagerService(env);
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles(); // allow reference to static files in wwwroot

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //ServeFromDirectory(app, env, "node_modules"); grab from external
                // intercepts requests that would match files built by webpack and dynamically builds those files on demand
                // browser is guaranteed to receive up-to-date build output
                // instance of webpack stays active and has partial compilation states pre-cached in memory
                // requires aspnet-webpack
                // change project path so webpack middleware knows where to look
                app.UseWebpackDevMiddleware(new Microsoft.AspNetCore.SpaServices.Webpack.WebpackDevMiddlewareOptions
                {
                    ProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp"),
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }

            app.UseGraphiQl("/api/graphql");

            app.UseMvc(routes =>
            {
                routes.MapRoute("graphql", "api/graphql",
                    defaults: new { controller = "GraphQL", action = "Post" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        // statically include specified path (used to expose npm_modules for dev) https://stackoverflow.com/a/39465629
        public void ServeFromDirectory(IApplicationBuilder app, IHostingEnvironment env, string path)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, path)),
                RequestPath = "/" + path
            });
        }

    }
}
