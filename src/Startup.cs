using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using backendProject.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace backendProject
{
    public class Startup
    {
        public static List<string> Readiness = new List<string>();
        public static string Issuer = "rics";
        public static string Audience = "backendProject";
        public static SecurityKey SecurityKey { get; set; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            AppEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment AppEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (AppEnvironment.EnvironmentName.Equals("Development"))
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

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

            services.AddAuthorization(options =>
                {
                    options.AddPolicy("IsAdmin", policy => policy.RequireClaim("admin", "true"));
                });

            services.AddControllers();

            services.AddMvc()
                .AddNewtonsoftJson(x =>
                    {
                        x.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                    }
                );

            Readiness.Add("Services Configure");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Task.Factory.StartNew(async () =>
            {
                using (var serviceScope = app.ApplicationServices.CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                    await context.Database.MigrateAsync();
                    await context.Database.OpenConnectionAsync();
                }

                Readiness.Add("Database Migration");
            });

            if (env.EnvironmentName.Equals("Development"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(isOriginAllowed: _ => true)
                        .AllowCredentials();
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Readiness.Add("Configure the HTTP request pipeline");
        }
    }
}