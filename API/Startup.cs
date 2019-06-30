using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.DependencyInjection;
using Models.Secrets.Repositories;
using Models.Tags.Repositories;
using Swashbuckle.AspNetCore.Swagger;

namespace API
{
    public class Startup
    {
        private const string DocsRoute = "secret-materials/docs";
        private const string DocName = "SPH-API";
        private const string DocTitle = "SampleTextHack API";
        private const string CorsPolicy = "AllowAnyPolicy";
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITagRepository, MongoTagRepository>();
            services.AddSingleton<ISecretRepository, MongoSecretRepository>();

            // Any CORS permission
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(CorsPolicy));
            });
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy, builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            
            services.AddMvc();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(DocName, new Info
                {
                    Version = "v1",
                    Title = DocTitle,
                    Description = "ASP.NET Core Web API"
                });
                options.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}/API.xml");
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            
            app.Map("/index", builder => app.UseStatusCodePagesWithReExecute("/index.html"));
            app.Map("/secret", builder => app.UseStatusCodePagesWithReExecute("/secret.html"));
            ;
            app.UseCors(CorsPolicy);
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();

            app.UseSwagger(options => options.RouteTemplate = $"{DocsRoute}/{{documentName}}/swagger.json");
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = DocsRoute;
                options.SwaggerEndpoint($"/{DocsRoute}/{DocName}/swagger.json", DocTitle);
            });
        }
    }
}