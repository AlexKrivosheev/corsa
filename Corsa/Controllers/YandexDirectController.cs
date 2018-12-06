using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corsa.Domain.Exceptions;
using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.Requests;
using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.Providers.YandexXml;
using Corsa.Domain.Moduls.RutimeModuls.Antigate;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using Corsa.Domain.Processing;
using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Processing.Serp;
using Corsa.Domain.Tasks;
using Corsa.Domain.Tasks.Modules;
using Corsa.Infrastructure;
using Corsa.Infrastructure.Logging;
using Corsa.Infrastructure.Tasks;
using Corsa.Infrastructure.Tracking;
using Corsa.Models;
using Corsa.Models.Moduls;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Corsa.Controllers
{
    public class YandexDirectController : Controller
    {
        private ISourceRepository _reposiotry;
        private IConfiguration _configuration;
        private UserManager<IdentityUser> _userManager;
        private ILoggerFactory _loggerFactory;
        private IProjectModuleRegistry _modulRegistry;
        private ILogger _logger;
        private RuntimeTaskService _runtimeQueue;
        private IServiceProvider _provider;
        IStringLocalizer<YandexDirectController> _controllerLocalizer;
        IStringLocalizer<Module> _modulelocalizer;
        RuntimeContext _context;
        public YandexDirectController(IStringLocalizer<YandexDirectController> controllerLocalizer, IStringLocalizer<Module> moduleLocalizer, IServiceProvider provider, ISourceRepository repository, RuntimeTaskService taskManager, IProjectModuleRegistry modulRegistry, IConfiguration configuration, UserManager<IdentityUser> userManager, ILoggerFactory loggerFactory)
        {
            _provider = provider;
            _controllerLocalizer = controllerLocalizer;
            _modulelocalizer = moduleLocalizer;
            _loggerFactory = loggerFactory;
            _reposiotry = repository;
            _configuration = configuration;
            _userManager = userManager;
            _modulRegistry = modulRegistry;
            _runtimeQueue = taskManager;
            _logger = _loggerFactory.CreateLogger(AppLogger.UserCategory);
            _context = new RuntimeContext(_provider, _reposiotry, null, null, User.GetUserId());
        }

        public async Task<IActionResult> Index(int id)
        {
            _runtimeQueue.EraseState(id);
            var modul = _modulRegistry.OpenModule<YandexDirectModule>(_context,YandexDirectModule.ModuleCode, id);            
            return View(new YanexDirectModuleViewModel(id, modul.Configuration, modul.Results));
        }

        [HttpGet]
        public IActionResult Create(int project)
        {
            YandexDirectConfig config = new YandexDirectConfig() { ProjectModule = new ProjectModule() { ProjectId = project} };
            try
            {
                var targetProject = _reposiotry.GetProject(project);
                if (targetProject == null)
                {
                    throw new UserException($"Project {project} isn't find");
                }
                config.ProjectModule.Project = targetProject;
            }
            catch (UserException exc)
            {
                _logger.LogError(exc.Message);
                TempData["message"] = exc.Message;
            }
            
            return View(new YanexDirectModuleViewModel(config) {
                AntigateModules = ProjectModuleRegistry.GetModules<AntigateTaskConfig, AntigateTaskResult>(_context, _modulRegistry, project),
                HttpModules = ProjectModuleRegistry.GetModules<HttpProviderRuntimeConfig, HttpProviderData>(_context, _modulRegistry, project)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(YanexDirectModuleViewModel viewModel)
        {
            var modul = _modulRegistry.CreateModule<YandexDirectModule>(_context,YandexDirectModule.ModuleCode);

            modul.CreateAndSave(viewModel.Config);
            
            return RedirectToAction("Config", "Project", new { Id = viewModel.Project });
        }

        [HttpPost]
        public IActionResult Edit(YanexDirectModuleViewModel viewModel)
        {
            var modul = _modulRegistry.OpenModule<YandexDirectModule>(_context,YandexDirectModule.ModuleCode, viewModel.Id);

            var result = modul.SaveConfig(viewModel.Config);
            if (result)
            {
                TempData["message"] = $" Config of {modul.Name} has been changed";
            }

            return View(new YanexDirectModuleViewModel(modul.Id, modul.Configuration)
            {
                AntigateModules = ProjectModuleRegistry.GetModules<AntigateTaskConfig, AntigateTaskResult>(_context, _modulRegistry, modul.Configuration.ProjectModule.Project.Id),
                HttpModules = ProjectModuleRegistry.GetModules<HttpProviderRuntimeConfig, HttpProviderData>(_context, _modulRegistry, modul.Configuration.ProjectModule.Project.Id)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var modul = _modulRegistry.OpenModule<YandexDirectModule>(_context,YandexDirectModule.ModuleCode, id);            
            return View(new YanexDirectModuleViewModel(id, modul.Configuration) {
                AntigateModules = ProjectModuleRegistry.GetModules<AntigateTaskConfig, AntigateTaskResult>(_context, _modulRegistry, modul.Configuration.ProjectModule.Project.Id),
                HttpModules = ProjectModuleRegistry.GetModules<HttpProviderRuntimeConfig, HttpProviderData>(_context, _modulRegistry, modul.Configuration.ProjectModule.Project.Id)
            });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var module = _modulRegistry.OpenModule<YandexDirectModule>(_context,YandexDirectModule.ModuleCode, id);
            module.Drop();
            return RedirectToAction("Config", "Project", new { id = module.Configuration.ProjectModule?.ProjectId });
        }

        public JsonResult GetModuleState(int id)
        {
            return new JsonResult(ModuleRuntimeDetails.CreateModuleExecStateDetails(_runtimeQueue[id]));
        }

        public JsonResult Update(int id, string request)
        {
            _runtimeQueue.EraseState(id);

            var task = new ProjectModuleTask<string, List<SerpWebPage>>(_provider, id,User.GetUserId(), request);
            
            _runtimeQueue.Add(task);
            return new JsonResult(ModuleRuntimeDetails.CreateModuleExecStateDetails(_runtimeQueue[id]));
        }

        public PartialViewResult GetResult(int modulId)
        {
            var modul = _modulRegistry.OpenModule<YandexDirectModule>(_context,YandexDirectModule.ModuleCode, modulId);

            var result = new ModuleTaskResult<List<SerpWebPage>>();
            var yandexTask = _runtimeQueue[modulId] as ProjectModuleTask<string, List<SerpWebPage>>;
            if (yandexTask != null)
            {
                result = yandexTask.Result;
            }

            if (result != null)
            {
                return PartialView("_ModuleResultDetails", result.Details);
            }
            else
            {
                return PartialView("_EmptyModuleResult");
            }
        }

        public PartialViewResult GetStatistics(int id)
        {            
            var result = new ModuleTaskResult<List<SerpWebPage>>();
            var yandexTask = _runtimeQueue[id] as ProjectModuleTask<string, List<SerpWebPage>>;
            if (yandexTask != null)
            {
                var modul = _modulRegistry.OpenModule<YandexDirectModule>(_context, YandexDirectModule.ModuleCode, yandexTask.ModuleId);

                result = yandexTask.Result;
                result.Details = modul.GetDetails(yandexTask.Result.Details.Id);
            }
            return PartialView("_Statistics", new ModuleResultViewModel<List<SerpWebPage>>(id,result));
        }
    }
}