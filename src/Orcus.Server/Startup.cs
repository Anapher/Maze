using System;
using System.Linq;
using System.Runtime.Loader;
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orcus.Server.Authentication;
using Orcus.Server.ControllersBase;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Extensions;
using Orcus.Server.Hubs;
using Orcus.Server.Library.Interfaces;
using Orcus.Server.Library.Services;
using Orcus.Server.Middleware;
using Orcus.Server.Options;
using Orcus.Server.OrcusSockets;
using Orcus.Server.Service.Connection;
using Orcus.Server.Service.Modules;
using Orcus.Server.Service.Modules.Config;
using Orcus.Server.Service.Modules.Loader;

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
            services.AddSingleton<DefaultTokenProvider>();

            var provider = services.BuildServiceProvider();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                options.TokenValidationParameters =
                    provider.GetService<DefaultTokenProvider>().GetValidationParameters());

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
            
            var containerBuilder = new ContainerBuilder();
            LoadModules(containerBuilder, provider.GetService<IOptions<ModulesOptions>>().Value).Wait();

            containerBuilder.RegisterType<ConnectionManager>().As<IConnectionManager>();
            containerBuilder.RegisterType<ModulePackageManager>().As<IModulePackageManager>();

            containerBuilder.Populate(services);

            var container = containerBuilder.Build();
            container.Execute<IStartupAction>().Wait();

            return new AutofacServiceProvider(container);
        }

        private async Task LoadModules(ContainerBuilder containerBuilder, ModulesOptions modulesOptions)
        {
            var modulesConfig = new ModulesConfig(modulesOptions.ModulesFile);
            var modulesLock = new ModulesLock(modulesOptions.ModulesLock);

            await modulesConfig.Reload();
            await modulesLock.Reload();

            var orcusProject = new OrcusProject(modulesOptions, modulesConfig, modulesLock);
            if (modulesConfig.Modules.Any())
            {
                var loader = new ModuleLoader(orcusProject, AssemblyLoadContext.Default);
                await loader.Load(modulesConfig.Modules, modulesLock.Modules[orcusProject.Framework]);

                loader.ModuleTypeMap.Configure(containerBuilder);

                containerBuilder.RegisterInstance(new ModuleControllerProvider(loader.ModuleTypeMap))
                    .AsImplementedInterfaces();
            }
            else
            {
                containerBuilder.RegisterInstance(new ModuleControllerProvider()).AsImplementedInterfaces();
            }

            containerBuilder.RegisterInstance(modulesConfig).AsImplementedInterfaces();
            containerBuilder.RegisterInstance(modulesLock).AsImplementedInterfaces();
            containerBuilder.RegisterInstance(orcusProject).AsImplementedInterfaces();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            app.UseMvc();
            app.UseSignalR(routes => routes.MapHub<AdministrationHub>("/v1/signalR"));
            app.Map("/ws", builder => builder.UseOrcusSockets().UseMiddleware<OrcusSocketManagerMiddleware>());

            app.ApplicationServices
                .Execute<IConfigureServerPipelineAction, IApplicationBuilder, IHostingEnvironment>(app, env).Wait();
        }
    }
}