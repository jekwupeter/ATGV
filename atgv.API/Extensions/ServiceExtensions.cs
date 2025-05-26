using atgv.Core.Entities;
using atgv.Core.Interfaces;
using atgv.Core.Utilities;
using atgv.Infrastructure;
using atgv.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace atgv.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddDbContextWithInMemory(this IServiceCollection services)
        {
           services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDb");
            });
        }

        public static void AddServiceDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, FakeEmailService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddScoped<IAccessTokenService, AccessTokenService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            services.AddSingleton<IHelpers, Helpers>();
        }

        public static void AddConfigSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(options => configuration.GetSection(nameof(JwtSettings)).Bind(options));
            services.Configure<SmtpSettings>(options => configuration.GetSection(nameof(SmtpSettings)).Bind(options));
            services.Configure<AccessTokenSettings>(options => configuration.GetSection(nameof(AccessTokenSettings)).Bind(options));
        }

        public static void AddCacheDependency(this IServiceCollection services)
        {
            services.AddMemoryCache();
        }
         public static void ConfigureSwaggerGen(this IServiceCollection services)
         {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ATGV API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme." +
                    " Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
         }

        public static void AddJWTRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSettings? jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings!.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization();
        }
    }  
}
