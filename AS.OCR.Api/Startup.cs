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
using System.Text;

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

            //支持SwaggerUI 并集成JWT  Bearer
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("OCR", new OpenApiInfo
                {
                    Title = "OCR API 2.0",
                    Version = "v2",
                    Description = "OCR API 2.0"
                });
                //启用auth支持
                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请在此输入您的Token，ex: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement{
                        {
                        new OpenApiSecurityScheme{
                            Reference=new OpenApiReference{
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{ }
                        }
                    });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);
            });

            //jwt认证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        ValidateLifetime = true,//是否验证失效时间
                        ClockSkew = TimeSpan.FromMinutes(30),
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidAudience = "http://localhost:5000",//Audience
                        ValidIssuer = "http://localhost:5000",//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationUtil.TokenKey))//拿到SecurityKey
                    };
                });

            services.AddAuthorization();

            services.AddMvcCore(s => s.Filters.Add(typeof(CustomActionFilter)))
                .AddRazorViewEngine();
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
            //认证
            app.UseAuthentication();
            //授权
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
