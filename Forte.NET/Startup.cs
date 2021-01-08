using GraphQL.Conventions;
using GraphQL.Server;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Query = Forte.NET.Schema.Query;

namespace Forte.NET {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            var engine = new GraphQLEngine()
                .WithQuery<Query>()
                .BuildSchema();
            var schema = engine.GetSchema();

            services.AddControllers();
            // services.AddSingleton(engine);
            services.AddSingleton(schema);
            // services.AddWebSockets(options => { });
            services
                .AddGraphQL()
                // .AddWebSockets()
                // .AddGraphTypes()
                .AddSystemTextJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseGraphQLPlayground(new() { Path = "/playground" });
            // app.UseWebSockets();
            // app.UseGraphQLWebSockets<ISchema>();
            // app.UseGraphiQLServer(new GraphiQLOptions { Path = "/graphiql" });
            app.UseGraphQL<ISchema>();
        }
    }
}
