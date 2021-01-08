using GraphQL.Conventions;
using GraphQL.Instrumentation;
using GraphQL.Server;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Query = Forte.NET.Schema.Query;

namespace Forte.NET {
    public class Startup {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Infer the schema using GraphQL.Conventions
            var engine = new GraphQLEngine()
                .WithQuery<Query>()
                .WithMiddleware<InstrumentFieldsMiddleware>()
                .BuildSchema();
            var schema = engine.GetSchema();

            services.AddControllers();
            services.AddSingleton(schema);
            services.AddWebSockets(_ => { });
            services
                .AddGraphQL((options, provider) => {
                    // This instrumentation setting is incompatible with
                    // GraphQL.Conventions and is handled via a different method.
                    // See https://github.com/graphql-dotnet/conventions/issues/211
                    options.EnableMetrics = false;
                    var logger = provider.GetRequiredService<ILogger<Startup>>();
                    options.UnhandledExceptionDelegate = ctx =>
                        logger.LogError(
                            "GraphQL Error: {Error}\n{Stacktrace}",
                            ctx.OriginalException.Message,
                            ctx.OriginalException.StackTrace
                        );
                })
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
            app.UseGraphQLWebSockets<ISchema>();
            app.UseGraphiQLServer(new GraphiQLOptions { Path = "/graphiql" });
            app.UseGraphQL<ISchema>();
        }
    }
}
