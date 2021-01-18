using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using StudentAccounting.Server.Domain;
using StudentAccounting.Server.Services;
using StudentAccounting.Server.Domain.Entities;
using System;
using AutoMapper;
using FluentValidation.AspNetCore;
using StudentAccounting.MailService;
using StudentAccounting.RazorClassLib.Services;
using Hangfire;
using StudentAccounting.Server.Helpers;


namespace StudentAccounting.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddTransient<EmailService>();
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

            services
                .AddIdentity<DbUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var jwtSection = Configuration.GetSection("JwtBearerTokenSettings");
            services.Configure<JwtBearerTokenSettings>(jwtSection);

            //var jwtBearerTokenSettings = jwtSection.Get<JwtBearerTokenSettings>();
            //services
            //    .AddAuthentication(options =>
            //    {
            //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    })
            //    .AddJwtBearer(options =>
            //    {
            //        options.RequireHttpsMetadata = false; // TODO: use ssl
            //        options.SaveToken = true;
            //        options.TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            ValidateIssuerSigningKey = true,
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            IssuerSigningKey = jwtBearerTokenSettings.GetSymmetricSecurityKey(),
            //            ClockSkew = TimeSpan.Zero
            //        };
            //    });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            services.AddHangfire(x => x.UseSqlServerStorage(
                                 Configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();

            services
                .AddControllersWithViews()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireDashboard();

            app.UseRouting();

            app.UseCors();

            app.UseMiddleware<JwtMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using var scope = app.ApplicationServices.CreateScope();
            Seeder.SetTestAdmin(scope.ServiceProvider, Configuration).Wait();
            //Seeder.SetTestUsers(scope.ServiceProvider, Configuration).Wait();
        }
    }
}
