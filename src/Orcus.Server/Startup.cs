using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orcus.ModuleManagement;
using Orcus.Server.AppStart;
using Orcus.Server.Authentication;
using Orcus.Server.Autofac;
using Orcus.Server.BusinessDataAccess;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Extensions;
using Orcus.Server.Hubs;
using Orcus.Server.Library.Interfaces;
using Orcus.Server.Library.Services;
using Orcus.Server.Middleware;
using Orcus.Server.Options;
using Orcus.Server.Service;
using Orcus.Server.Service.Connection;
using Orcus.Server.Service.Modules;
using Orcus.Sockets;

namespace Orcus.Server
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<ModulesOptions>(Configuration.GetSection("Modules"));
            services.Configure<AuthenticationOptions>(Configuration.GetSection("Authentication"));
            services.Configure<OrcusSocketOptions>(Configuration.GetSection("Socket"));
            services.Configure<ModulePackageManagerOptions>(Configuration.GetSection("Modules.PackageManager"));

            services.AddSingleton<ITokenProvider, DefaultTokenProvider>();
            services.AddLogging();
            services.AddMemoryCache();

            var provider = services.BuildServiceProvider();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                options.TokenValidationParameters =
                    provider.GetService<ITokenProvider>().GetValidationParameters());

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy("admin", builder => builder.RequireRole("admin"));
                options.AddPolicy("installModules", builder => builder.RequireRole("installingUser"));
            });

            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.Configure());
            services.AddEntityFrameworkSqlite().AddDbContext<AppDbContext>(builder =>
                builder.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSignalR();

            Mapper.Initialize(options => options.AddProfile<AutoMapperProfile>());
            
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<ConnectionManager>().As<IConnectionManager>().SingleInstance();
            containerBuilder.RegisterType<ModulePackageManager>().As<IModulePackageManager>().SingleInstance();
            containerBuilder.RegisterType<CommandDistributer>().As<ICommandDistributer>().SingleInstance();

            containerBuilder
                .RegisterModule<DataAccessModule>()
                .RegisterModule<BusinessLogicModule>()
                .RegisterModule<ModuleManagementModule>();

            new PackageInitialization(provider.GetService<IOptions<ModulesOptions>>().Value, services)
                .LoadModules(containerBuilder).Wait();

            containerBuilder.Populate(services);

            var container = containerBuilder.Build();
            container.Execute<IStartupAction>().Wait();

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var appContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appContext.Database.Migrate();
                }
            }

            app.UseAuthentication();
            app.UseMvc();
            app.UseSignalR(routes => routes.MapHub<AdministrationHub>("/signalR"));
            app.Map("/ws", builder => builder.UseWebSockets().UseMiddleware<OrcusSocketManagerMiddleware>());

            app.ApplicationServices.Execute<IConfigureServerPipelineAction, PipelineInfo>(new PipelineInfo(app, env))
                .Wait();
        }
    }
}