﻿using System;
using System.Linq;
using System.Text;
using Autofac;
using Jimu.ApiGateway.Model;
using Jimu.Client;
using Jimu.Client.ApiGateway;
using Jimu.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace Jimu.ApiGateway
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
            services.AddCors();
            services.UseJimuSwagger(new Client.ApiGateway.SwaggerIntegration.JimuSwaggerOptions("My API"));
            services.UseJimu();

            /********** below is the jwt just for local apigateway controller not for the services

            var jwtOptions = Configuration.GetSection("JwtOptions").Get<JwtOptions>();
            // The key length needs to be of sufficient length, or otherwise an error will occur.
            var tokenSecretKey = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                // Token signature will be verified using a private key.
                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenSecretKey),

                // Token will only be valid if contains "accelist.com" for "iss" claim.
                ValidateIssuer = jwtOptions.ValidateIssuer,
                ValidIssuer = jwtOptions.ValidIssuer,

                // Token will only be valid if contains "accelist.com" for "aud" claim.
                ValidateAudience = jwtOptions.ValidateAudience,
                ValidAudience = jwtOptions.ValidAudience,

                // Token will only be valid if not expired yet, with 5 minutes clock skew.
                ValidateLifetime = jwtOptions.ValidateLifetime,
                RequireExpirationTime = false,
                ClockSkew = jwtOptions.TimeSpanClockSkew,

                ValidateActor = false
            };
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = tokenValidationParameters;
        });
            services.AddTransient(provider => new AuthService(tokenSecretKey));
            
             *********************/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseStaticFiles();
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            app.UseStatusCodePages();
            app.UseJimuSwagger();

            // jimu client
            var host = new ApplicationClientBuilder(new ContainerBuilder())
                //.UseLog4netLogger(new JimuLog4netOptions
                ////.UseNLogger(new LogOptions
                //{
                //    EnableConsoleLog = true,
                //    EnableFileLog = true,
                //    FileLogLevel = LogLevel.Info | LogLevel.Error,
                //})
                //.UseConsulForDiscovery(new Client.Discovery.ConsulIntegration.ConsulOptions("127.0.0.1", 8500, "hello"))
                //.UseDotNettyForTransfer()
                //.UseHttpForTransfer()
                //.UsePollingAddressSelector()
                //.UseServerHealthCheck(1)
                //.SetDiscoveryAutoUpdateJobInterval(new Client.Discovery.Implement.DiscoveryOptions(1))
                //.UseToken(() => { var headers = JimuHttpContext.Current.Request.Headers["Authorization"]; return headers.Any() ? headers[0] : null; })
                //.UseJoseJwtForOAuth<HttpAddress>(new Client.Auth.JwtAuthorizationOptions()
                //{
                //    ServerIp = "127.0.0.1",
                //    ServerPort = 5001,
                //    SecretKey = "test",
                //    ExpireTimeSpan = new TimeSpan(1, 0, 0),
                //    TokenEndpointPath = "/api/client/token",
                //    ValidateLifetime = true,
                //    CheckCredential = o =>
                //    {
                //        if (o.UserName == "admin" && o.Password == "admin")
                //        {
                //            o.AddClaim("department", "IT部");
                //        }
                //        else
                //        {
                //            o.Rejected("401", "acount or password incorrect");
                //        }
                //    }
                //})
                .Build();
            app.UseJimu(host);
            host.Run();
        }
    }
}
