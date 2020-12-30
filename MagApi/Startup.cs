using MagApi.Exceptions;
using MagApi.Identity;
using MagApi.Identity.Helpers;
using MagApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagApi
{
    public class Startup
    {
        readonly string MagCORSPolicy = "_MagCORSPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITokenHelper, TokenHelper>();

            services.AddDbContext<MagIdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MagIdentityContextConnection"))
            );

            services.AddDbContext<MagDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MagContextConnection"))
            );

            services.AddIdentity<MagApplicationUser, MagApplicationRole>(options =>
                        {
                            options.Password.RequireDigit = false;
                            options.Password.RequiredLength = 6;
                            options.Password.RequireLowercase = false;
                            options.Password.RequireNonAlphanumeric = false;
                            options.Password.RequiredUniqueChars = 1;

                            options.SignIn.RequireConfirmedAccount = false;
                            options.SignIn.RequireConfirmedEmail = false;
                            options.SignIn.RequireConfirmedPhoneNumber = false;

                        })
                    .AddEntityFrameworkStores<MagIdentityDbContext>();

            // The following return 404 on secured apis. Need to explicitly set schemes
            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["Jwt:Issuer"],
                            ValidAudience = Configuration["Jwt:Issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                        };
                    });

            services.AddCors(options =>
            {
                options.AddPolicy(name: MagCORSPolicy,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin()
                                            .WithHeaders("origin", "cache-control", "content-disposition", "content-type", "accept", "authorization", "forwarded", "x-forwarded-host", "x-forwarded-for", "x-forwarded-proto")
                                            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS", "HEAD")
                                            .WithExposedHeaders("content-disposition", "content-type")
                                            .SetPreflightMaxAge(new TimeSpan(1209600));
                                  });
            });

            services.AddControllers();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseCors(MagCORSPolicy);

            // app.UseResponseCaching();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
