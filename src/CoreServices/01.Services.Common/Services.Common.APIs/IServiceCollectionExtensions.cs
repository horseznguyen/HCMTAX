using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Common.APIs.Filters;
using Services.Common.SecurityUtils;
using Services.Common.SecurityUtils.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services.Common.APIs
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, List<Assembly> moduleTypes, string assemblyName = "API")
        {
            if (moduleTypes == null || !moduleTypes.Any()) return services;

            var executingAssembly = moduleTypes.FirstOrDefault(x => x.FullName.Contains(assemblyName));

            if (executingAssembly != null)
            {
                services.AddSwaggerGen(c =>
                {
                    c.DocumentFilter<HideOcelotFilter>();

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme."
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
                            new string[] {}
                        }
                    });

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{executingAssembly.GetName().Name}.xml";

                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                    c.IncludeXmlComments(xmlPath);
                });
            }

            return services;
        }

        public static IServiceCollection AddCustomApiVersion(this IServiceCollection services, Action<ApiVersioningOptions> apiVersionOptions)
        {
            if (apiVersionOptions == null)
            {
                apiVersionOptions = opt =>
                {
                    opt.ReportApiVersions = true;
                    opt.AssumeDefaultVersionWhenUnspecified = true;
                    opt.DefaultApiVersion = new ApiVersion(1, 0);
                };
            }

            services.AddApiVersioning(apiVersionOptions);

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            var jwtOptions = AppSettings.Instance.Get<JwtOptions>(nameof(JwtOptions));

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                jwtOptions.SecretKey = SecurityHelper.Decrypt(jwtOptions.SecretKey, Environment.GetEnvironmentVariable("tax_secure_key", EnvironmentVariableTarget.Machine));

                jwtOptions.Issuer = SecurityHelper.Decrypt(jwtOptions.Issuer, Environment.GetEnvironmentVariable("tax_secure_key", EnvironmentVariableTarget.Machine));

                jwtOptions.Audience = SecurityHelper.Decrypt(jwtOptions.Audience, Environment.GetEnvironmentVariable("tax_secure_key", EnvironmentVariableTarget.Machine));
            }

            services.Configure<JwtOptions>(_ =>
            {
                _.SecretKey = jwtOptions.SecretKey;

                _.Issuer = jwtOptions.Issuer;

                _.Audience = jwtOptions.Audience;

                _.RefreshTokenExpiryMinutes = jwtOptions.RefreshTokenExpiryMinutes;

                _.LengthOfRefreshToken = jwtOptions.LengthOfRefreshToken;

                _.ValidFor = jwtOptions.ValidFor;
            });

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.SecretKey));

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,

                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = signingKey
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }
    }
}