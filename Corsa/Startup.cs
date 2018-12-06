using Corsa.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Corsa.Models;
using Microsoft.AspNetCore.Identity;
using Corsa.Domain.Models.Requests;
using Microsoft.Extensions.Logging;
using Corsa.Infrastructure.Logging;
using Corsa.Domain.Logging;
using Corsa.Domain.Processing.Moduls;
using Corsa.Models.Moduls;
using Corsa.Domain.Moduls.SerpAnalysis;
using Corsa.Domain.Tasks;
using Corsa.Infrastructure.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Corsa.Domain.Moduls.Providers.YandexXml;
using Corsa.Domain.Processing.Moduls.RutimeModuls;
using Corsa.Domain.Moduls.LexicalAnalysis;
using Corsa.Domain.Tasks.Modules;
using Corsa.Domain.Moduls.RutimeModuls.EmailNotif;
using System.Collections.Generic;
using System;
using Corsa.Domain.Moduls;

namespace Corsa
{
    public class Startup
    {

        private const string enUsCulture = "en-US";
        private const string ruRUCulture = "ru-RU";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                  options.UseSqlServer(Configuration["Data:ConnectionStrings:CorsaRequesTool"]                
                );
            }
            );

            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(
            Configuration["Data:CorsaIdentity:ConnectionString"]));

            services.AddDbContext<AppLogDbContext>(options => options.UseSqlServer(
            Configuration["Data:ConnectionStrings:CorsaLogging"] ));

            services.AddIdentity<IdentityUser, IdentityRole>(opts=>{
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();
            services.AddTransient<ISourceRepository, EFPRequestReprository>();
            services.AddTransient<ILogReprository, AppLogRepository>();
            services.AddSingleton<IProjectModuleRegistry, ProjectModuleRegistry>();
                        
            services.AddSingleton<ProjectModuleViewRegistry>();

            services.AddSingleton<ITaskPipline, TaskPipline>();
            services.AddSingleton<IScheduleTaskQueue, ScheduleTaskQueue>();

            services.AddSingleton<RuntimeTaskService>();            

            services.AddHostedService<TaskPiplineService>();
            services.AddHostedService<ScheduleService>();
            services.AddHostedService<ScheduleBuilderService>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddSession();
            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseBrowserLink();
            //app.UseMvcWithDefaultRoute();

            app.UseDeveloperExceptionPage();            
            app.UseStaticFiles();
            app.UseStatusCodePages();
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc(routes => {

                routes.MapRoute(name: "default", template: "{controller=Project}/{action=Index}/{id?}");
            });

            var repository = app.ApplicationServices.GetService<ISourceRepository>();

            loggerFactory.AddProvider(new AppLogProvider(app.ApplicationServices, (catergory, level) => { return string.Equals(AppLogger.UserCategory, catergory); }));            
            
            RegisterProjectModules(app.ApplicationServices.GetService<IProjectModuleRegistry>());
            
            RegisterProjectModuleControllers(app.ApplicationServices.GetService<ProjectModuleViewRegistry>());

            ConfigureCulture(app);

            EFPRequestReprository.EnsurePopulated(app, app.ApplicationServices.GetService<AppIdentityDbContext>(),Configuration);
            AppIdentityDbContext.EnsurePopulated(app);            
        }

        private void RegisterProjectModules(IProjectModuleRegistry registry)
        {
            registry.RegisterModule(LexModule.ModuleCode, new LexModuleFactory(),typeof(ProjectModuleTask<LexModuleData>));
            registry.RegisterModule(SerpModule.ModuleCode, new SerpModuleFactory(), typeof(ProjectModuleTask<SerpModuleData>));
            registry.RegisterModule(YandexXMLModule.ModuleCode,  new YandexXmlFactory());
            registry.RegisterModule(YandexDirectModule.ModuleCode,  new YandexDirectFactory());
            registry.RegisterModule(HttpProviderModule.ModuleCode, new HttpProviderFactory());
            registry.RegisterModule(AntigateModule.ModuleCode, new AntigateFactory());
            registry.RegisterModule(EmailNotifModule.ModuleCode, new EmailNotifFactory(), null, new Dictionary<Type, Type>()
                {
                    { typeof(ModuleTaskResult<SerpModuleData>),typeof(ProjectModuleTask<ModuleTaskResult<SerpModuleData>, NotifResult>)},
                    { typeof(ModuleTaskResult<LexModuleData>),typeof(ProjectModuleTask<ModuleTaskResult<LexModuleData>, NotifResult>)}
                } 
            );
        }

        private void RegisterProjectModuleControllers(ProjectModuleViewRegistry registry)
        {
            registry.Register(LexModule.ModuleCode, new ProjectModuleViewDescriptor("Lexical Analysis","Lexial analysis of search engine serp", "LexicalAnalysis","Create","Edit","Index", "~/Views/LexicalAnalysis/_ProjectShowcase.cshtml", "~/Views/LexicalAnalysis/_ToolboxShowcase.cshtml",false));
            registry.Register(SerpModule.ModuleCode, new ProjectModuleViewDescriptor("Serp Analysis", "Search engine serp", "SerpAnalysis", "Create", "Edit", "Index", "~/Views/SerpAnalysis/_ProjectShowcase.cshtml", "~/Views/SerpAnalysis/_ToolboxShowcase.cshtml",false));
            registry.Register(YandexXMLModule.ModuleCode, new ProjectModuleViewDescriptor("Yandex XML", "Module", "YandexXML", "Create", "Edit", "Index", "~/Views/YandexXML/_ProjectShowcase.cshtml", "~/Views/YandexXML/_ToolboxShowcase.cshtml",true));
            registry.Register(YandexDirectModule.ModuleCode, new ProjectModuleViewDescriptor("Yandex Direct", "Module", "YandexDirect", "Create", "Edit", "Index", "~/Views/YandexDirect/_ProjectShowcase.cshtml", "~/Views/YandexDirect/_ToolboxShowcase.cshtml", true));
            registry.Register(HttpProviderModule.ModuleCode, new ProjectModuleViewDescriptor("Http Provider", "Module", "HttpProvider", "Create", "Edit", "Index", "~/Views/HttpProvider/_ProjectShowcase.cshtml", "~/Views/HttpProvider/_ToolboxShowcase.cshtml", true));
            registry.Register(AntigateModule.ModuleCode, new ProjectModuleViewDescriptor("Antigate", "Module", "Antigate", "Create", "Edit", "Index", "~/Views/Antigate/_ProjectShowcase.cshtml", "~/Views/Antigate/_ToolboxShowcase.cshtml", true));
            registry.Register(EmailNotifModule.ModuleCode, new ProjectModuleViewDescriptor("Email Notification", "Module", "EmailNotif", "Create", "Edit", "Index", "~/Views/EmailNotif/_ProjectShowcase.cshtml", "~/Views/EmailNotif/_ToolboxShowcase.cshtml", true));            
        }

        private void ConfigureCulture(IApplicationBuilder app)
        {
            var supportedCultures = new[]
{
                new CultureInfo(enUsCulture),
                new CultureInfo(ruRUCulture),
};

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(enUsCulture),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });


        }

    }
}
