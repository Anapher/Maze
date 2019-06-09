using System;
using System.Collections.Generic;
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
using Microsoft.Extensions.Options;
using Maze.ModuleManagement;
using Maze.Server.AppStart;
using Maze.Server.Authentication;
using Maze.Server.Autofac;
using Maze.Server.BusinessDataAccess;
using Maze.Server.Connection.Utilities;
using Maze.Server.Data.EfCode;
using Maze.Server.Extensions;
using Maze.Server.Library.Hubs;
using Maze.Server.Library.Interfaces;
using Maze.Server.Library.Services;
using Maze.Server.Middleware;
using Maze.Server.Options;
using Maze.Server.Service;
using Maze.Server.Service.Connection;
using Maze.Server.Service.Modules;
using Maze.Sockets;
using Serilog;
using System.Reflection;

namespace Maze.Server
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
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
            Log.Information("Maze.Server Version {0}", Assembly.GetEntryAssembly().GetInformationalVersion());
            Log.Information("Starting web host");

            services.Configure<ModulesOptions>(Configuration.GetSection("Modules"));
            services.Configure<AuthenticationOptions>(Configuration.GetSection("Authentication"));
            services.Configure<MazeSocketOptions>(Configuration.GetSection("Socket"));
            services.Configure<ModulePackageManagerOptions>(Configuration.GetSection("Modules.PackageManager"));

            services.AddSingleton<ITokenProvider, DefaultTokenProvider>();
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

            var mcvBuilder = services.AddMvc().AddJsonOptions(options => options.SerializerSettings.Configure());
            services.AddEntityFrameworkSqlite().AddDbContext<AppDbContext>(builder =>
                builder.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSignalR();
            
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<ConnectionManager>().As<IConnectionManager>().SingleInstance();
            containerBuilder.RegisterType<ModulePackageManager>().As<IModulePackageManager>().SingleInstance();
            containerBuilder.RegisterType<CommandDistributor>().As<ICommandDistributer>().SingleInstance();
            containerBuilder.RegisterType<XmlSerializerCache>().As<IXmlSerializerCache>().SingleInstance();

            containerBuilder
                .RegisterModule<DataAccessModule>()
                .RegisterModule<BusinessLogicModule>()
                .RegisterModule<ModuleManagementModule>();

            new PackageInitialization(provider.GetService<IOptions<ModulesOptions>>().Value, services, Configuration)
                .LoadModules(containerBuilder, mcvBuilder).Wait();

            containerBuilder.Populate(services);

            var container = containerBuilder.Build();
            Mapper.Initialize(options =>
            {
                options.AddProfile<AutoMapperProfile>();
                options.AddProfile<ServerAutoMapperProfile>();

                foreach (var autoMapperProfile in container.Resolve<IEnumerable<Profile>>())
                {
                    options.AddProfile(autoMapperProfile);
                }
            });

            container.Execute<IStartupAction>().Wait();

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            app.UseMvc();
            app.UseSignalR(routes => routes.MapHub<AdministrationHub>("/signalR"));
            app.Map("/ws", builder => builder.UseWebSockets().UseMiddleware<MazeSocketManagerMiddleware>());

            app.ApplicationServices.Execute<IConfigureServerPipelineAction, PipelineInfo>(new PipelineInfo(app, env))
                .Wait();
        }
    }
}