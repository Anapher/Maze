using System;
using System.Globalization;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Server.Authentication;
using Orcus.Server.Data.EfCode;

namespace Orcus.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var authenticationConfig = Configuration.GetSection("Authentication");
            var tokenProvider = new DefaultTokenProvider(authenticationConfig["Issuer"],
                authenticationConfig["Audience"], Convert.FromBase64String(authenticationConfig["Secret"]),
                TimeSpan.FromHours(double.Parse(authenticationConfig["AccountTokenValidityHours"],
                    CultureInfo.InvariantCulture)));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Path.Value.EndsWith("signalR"))
                            if (context.Request.Query.TryGetValue("signalRTokenHeader", out var token))
                                context.Token = token;
                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = tokenProvider.GetValidationParameters();
            });
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddMvc();
            services.AddEntityFrameworkSqlite().AddDbContext<AppDbContext>(builder =>
                builder.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(tokenProvider).AsImplementedInterfaces();

            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}