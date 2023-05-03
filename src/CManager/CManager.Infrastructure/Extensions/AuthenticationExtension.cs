﻿using CManager.Infrastructure.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CManager.Infrastructure.Extensions
{
    public static class AuthenticationExtension
    {
        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration, JwtOptions jwtOptions)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.SecurityKey));

            services.Configure<JwtOptions>(options =>
            {
                options.Issuer = jwtOptions.Issuer;
                options.Audience = jwtOptions.Audience;
                options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
                options.AccessTokenExpiration = jwtOptions.AccessTokenExpiration;
                options.RefreshTokenExpiration = jwtOptions.RefreshTokenExpiration;
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration.GetSection($"{nameof(JwtOptions)}:{nameof(JwtOptions.Issuer)}").Value,

                ValidateAudience = true,
                ValidAudience = configuration.GetSection($"{nameof(JwtOptions)}:{nameof(JwtOptions.Audience)}").Value,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            // var tokenValidationParametersGoogle = new TokenValidationParameters
            // {
            //     ValidateIssuer = true,
            //     ValidIssuer = configuration.GetSection($"{nameof(JwtOptions)}:{nameof(JwtOptions.Issuer)}").Value,

            //     ValidateAudience = true,
            //     ValidAudience = configuration.GetSection($"{nameof(JwtOptions)}:{nameof(JwtOptions.Audience)}").Value,

            //     ValidateIssuerSigningKey = true,
            //     IssuerSigningKey = securityKey,

            //     RequireExpirationTime = true,
            //     ValidateLifetime = true,

            //     ClockSkew = TimeSpan.Zero
            // };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            })
            // .AddJwtBearer(options =>
            // {
            //     options.TokenValidationParameters = tokenValidationParametersGoogle;
            // })
            ;
        }
    }
}
