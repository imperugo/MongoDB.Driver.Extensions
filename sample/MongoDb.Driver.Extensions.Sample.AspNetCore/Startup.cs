using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.Driver.Extensions.Sample.AspNetCore.Data;
using MongoDb.Driver.Extensions.Sample.AspNetCore.Data.Documents;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;

namespace MongoDb.Driver.Extensions.Sample.AspNetCore
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
            var dbConfiguration = new MongoDbDatabaseConfiguration();
            Configuration
                .GetSection("MongoDb")
                .Bind(dbConfiguration);

            services.AddMongoDbRepository(dbConfiguration);

            services.AddSingleton<IRepository<User, string>, UserRepository>();
            services.AddSingleton<IMongoDbNamingHelper, OverrideNamingConvention>();

            services.AddMvc(opt => opt.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}