using System;
using AS.OCR.Api.Middleware;
using AS.OCR.Commom.Util;
using Exceptionless;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;

namespace AS.OCR.Api
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
            services.AddControllers();

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("OCR", new OpenApiInfo
                {
                    Title = "OCR API 2.0",
                    Version = "v2",
                    Description = "OCR API 2.0"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);
                //s.AddSecurityDefinition("Bearer", new OpenApiseSchema
                //{
                //    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                //    Name = "Authorization",//Jwt default param name
                //    In = "header",//Jwt store address
                //    Type = "apiKey"//Security scheme type
                //});
                //Add authentication type
                //s.AddSecurityRequirement(new OpenApiSecurityRequirement("Bearer", new string[] { }));
            });

            //services.AddAuthentication(s =>
            // {
            //     //2、Authentication
            //     s.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //     s.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //     s.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // }).AddJwtBearer(s =>
            // {
            //     //3、Use Jwt bearer 
            //     s.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         ValidIssuer = issuer,
            //         ValidAudience = audience,
            //         IssuerSigningKey = key,
            //         ClockSkew = expiration,
            //         ValidateLifetime = true
            //     };
            //     s.Events = new JwtBearerEvents
            //     {
            //         OnAuthenticationFailed = context =>
            //         {
            //             //Token expired
            //             if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            //             {
            //                 context.Response.Headers.Add("Token-Expired", "true");
            //             }
            //             return Task.CompletedTask;
            //         }
            //     };
            // });

            services.AddMvcCore().AddRazorViewEngine();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware(typeof(CustomExceptionHandlerMiddleware));//全局异常

            ExceptionlessClient.Default.Configuration.ApiKey = ConfigurationUtil.Exceptionless_ApiKey;
            ExceptionlessClient.Default.Configuration.ServerUrl = ConfigurationUtil.Exceptionless_ServerUrl;
            app.UseExceptionless();
            //Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            //Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint("/swagger/OCR/swagger.json", "OCR Docs");
                option.RoutePrefix = string.Empty;
                option.DocumentTitle = "OCR API";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //更改默认静态文件目录
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Script")),
                RequestPath = "/Script"
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
