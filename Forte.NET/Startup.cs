using Forte.NET.Database;
using Forte.NET.Schema;
using GraphQL.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Forte.NET {
    public class Startup {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddDbContext<ForteDbContext>();
            services.AddSingleton<ForteSchema>();
            services.AddWebSockets(_ => { });
            services
                .AddGraphQL((options, provider) => {
                    var logger = provider.GetRequiredService<ILogger<Startup>>();
                    options.UnhandledExceptionDelegate = ctx =>
                        logger.LogError(
                            "GraphQL Error: {Error}",
                            ctx.Exception
                        );
                })
                .AddGraphTypes(typeof(ForteSchema))
                .AddRelayGraphTypes()
                .AddUserContextBuilder<ForteDbContext>()
                .AddWebSockets()
                .AddSystemTextJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseWebSockets();
            app.UseGraphQLWebSockets<ForteSchema>();
            app.UseGraphiQLServer(new() { Path = "/graphiql" });
            app.UseGraphQL<ForteSchema>();
        }
    }
}
