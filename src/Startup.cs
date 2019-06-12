using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backendProject.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace backendProject
{
    public class Startup
    {
        public static string Issuer = "rics";
        public static string Audience = "backendProject";
        public static SecurityKey SecurityKey { get; set; }
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            AppEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment AppEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (AppEnvironment.IsDevelopment())
            {
                Console.WriteLine("Development Mode");
                services
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite($"Data Source=sqlite.db")
                    );

                SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("BllqVlGsFgKUchzUo5n7cQ=="));
            }
            else
            {
                Console.WriteLine("Production Mode");
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseMySql(Environment.GetEnvironmentVariable("ConnectionMySql")));

                SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("SecretKey")));
            }

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Startup.Issuer,
                        ValidAudience = Startup.Audience,
                        IssuerSigningKey = Startup.SecurityKey,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(x => x.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat);

            services.AddAuthorization(options =>
                {
                    options.AddPolicy("IsAdmin", policy => policy.RequireClaim("admin", "true"));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Task.Factory.StartNew(async () =>
            {
                using (var serviceScope = app.ApplicationServices.CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                    await context.Database.MigrateAsync();
                    await context.Database.OpenConnectionAsync();
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(isOriginAllowed: _ => true)
                        .AllowCredentials();
            });

            app.UseAuthentication();
            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}