using System;
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
using Orcus.Server.Data.EfCode;
using Orcus.Server.Extensions;
using Orcus.Server.Hubs;
using Orcus.Server.Middleware;
using Orcus.Server.Options;
using Orcus.Server.OrcusSockets;
using Orcus.Server.Service.Connection;
using Orcus.Server.Service.Modules;
using Orcus.Server.Service.Modules.Config;

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
            LoadModules(containerBuilder, provider.GetService<IOptions<ModulesOptions>>()).Wait();

            containerBuilder.RegisterType<ConnectionManager>().As<IConnectionManager>();
            containerBuilder.RegisterType<ModulePackageManager>().As<IModulePackageManager>();

            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        private async Task LoadModules(ContainerBuilder containerBuilder, IOptions<ModulesOptions> modulesOptions)
        {
            var modulesConfig = new ModulesConfig(modulesOptions.Value.ModulesFile);
            var modulesLock = new ModulesLock(modulesOptions.Value.ModulesLock);

            await modulesConfig.Reload();
            await modulesLock.Reload();

            var orcusProject = new OrcusProject(modulesOptions.Value.PrimarySources,
                modulesOptions.Value.DependencySources, modulesOptions.Value.Directory, modulesConfig, modulesLock);

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
        }
    }
}