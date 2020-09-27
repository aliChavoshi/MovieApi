using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoviesApi.Entities;
using MoviesApi.Filters;
using MoviesApi.Helpers;
using MoviesApi.Services;

namespace MoviesApi
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
            //IOC
            services.AddDbContextPool<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                        sqlServer => sqlServer.UseNetTopologySuite());
                }
            );

            services.AddCors(options =>
            {
                options.AddPolicy("AllowRequestIO",
                      builder => builder.WithOrigins("http://apirequest.io")
                     .AllowAnyMethod()
                     .AllowAnyHeader());
            });
            //Encrypted
            services.AddDataProtection();

            //AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfiles));


            //Hashing
            services.AddTransient<HashService>();


            //Save To Azure
            /*services.AddTransient<IFileStorageService, AzureStorageService>();*/

            //Save To Local
            services.AddTransient<IFileStorageService, InAppStorageService>();


            //Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //HATEOAS
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<GenreHateoasAttribute>();
            services.AddTransient<LinksGenerator>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //دسترسی صادر کننده
                        ValidateIssuer = false,
                        //دسترسی حضار
                        ValidateAudience = false,
                        //عمر ما را تایید کند
                        ValidateLifetime = true,
                        //کلید امضای خود را تایید کند
                        ValidateIssuerSigningKey = true,
                        //کلید امضای صادر کننده
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                        ClockSkew = TimeSpan.Zero
                    }
                );


            //این بخش در InAppStorageService  تزریق شده است 
            //پس باید اینجا قرار داشته باشد تا در اون کلاس تزریق شود
            services.AddHttpContextAccessor();

            //Learning
            services.AddSingleton<IRepository, InMemoryRepository>();

            //MyExceptionFilters
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyExceptionFilters));
            }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

            //Cashe
            services.AddResponseCaching();

            /*//Identity
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();*/

            //CustomFilters
            services.AddTransient<MyActionFilter>();

            //Auto Change INTheaters
            services.AddTransient<IHostedService, MovieInTheatersService>();

            /*//Write To File Start Stop
            services.AddTransient<IHostedService, WriteToFileHostedService>();*/

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MoviesAPI",
                    Description = "This is a Web API for Movies operations",
                    TermsOfService = new Uri("https://udemy.com/user/felipegaviln/"),
                    License = new OpenApiLicense()
                    {
                        Name = "MIT"
                    },
                    Contact = new OpenApiContact()
                    {
                        Name = "Felipe Gavilلn",
                        Email = "felipe_gavilan887@hotmail.com",
                        Url = new Uri("https://gavilan.blog/")
                    }
                });

                //For Comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "MoviesAPI");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /*app.UseCors(builder=>builder.WithOrigins("http://apirequest.io").WithMethods("GET","POST","PUT").AllowAnyHeader());*/

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            //Identity
            app.UseAuthentication();

            app.UseAuthorization();
            //Cors
            app.UseCors();
            //Cache
            app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
