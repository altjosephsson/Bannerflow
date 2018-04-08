using AutoMapper;
using Bannerflow.Api.Data;
using Bannerflow.Api.Infrastructure;
using Bannerflow.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Bannerflow.Api
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
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Banner, BannerDto>().ReverseMap();
            });

            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            

            var mongoClient = new MongoClient(Configuration.GetSection("MongoConnection:ConnectionString").Value);
            var database = mongoClient.GetDatabase(Configuration.GetSection("MongoConnection:Database").Value);

            services.AddScoped<IMongoDatabase>(_ => database);
            services.AddScoped<IMongoDbContext, MongoDbContext>();
            services.AddTransient<IBannerRepository, BannerRepository>();
            services.AddTransient<ITransformer, Transformer>();

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
