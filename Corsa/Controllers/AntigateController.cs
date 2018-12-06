using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Corsa.Domain.Processing.Moduls.RutimeModuls;
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
    public class AntigateController : Controller
    {
        private ISourceRepository _reposiotry;
        private IConfiguration _configuration;
        private UserManager<IdentityUser> _userManager;
        private ILoggerFactory _loggerFactory;
        private IProjectModuleRegistry _moduleRegistry;
        private ILogger _logger;
        private RuntimeTaskService _runtimeQueue;
        private IServiceProvider _provider;
        IStringLocalizer<AntigateController> _controllerLocalizer;
        IStringLocalizer<Module> _modulelocalizer;
        RuntimeContext _context;

        public AntigateController(IStringLocalizer<AntigateController> controllerLocalizer, IStringLocalizer<Module> moduleLocalizer, IServiceProvider provider, ISourceRepository repository, RuntimeTaskService taskManager, IProjectModuleRegistry modulRegistry, IConfiguration configuration, UserManager<IdentityUser> userManager, ILoggerFactory loggerFactory)
        {
            _provider = provider;
            _controllerLocalizer = controllerLocalizer;
            _modulelocalizer = moduleLocalizer;
            _loggerFactory = loggerFactory;
            _reposiotry = repository;
            _configuration = configuration;
            _userManager = userManager;
            _moduleRegistry = modulRegistry;
            _runtimeQueue = taskManager;
            _logger = _loggerFactory.CreateLogger(AppLogger.UserCategory);
            _context = new RuntimeContext(_provider, _reposiotry, null, null, User.GetUserId());
        }

        public async Task<IActionResult> Index(int id)
        {
            _runtimeQueue.EraseState(id);
            var modul = _moduleRegistry.OpenModule<AntigateModule>(_context, AntigateModule.ModuleCode, id);        
            



            return View(new AntigateModulViewModel(id, modul.Configuration, new List<ModuleTaskResult<AntigateTaskResult>>()));
        }

        [HttpGet]
        public IActionResult Create(int project)
        {
            AntigateConfig config = new AntigateConfig() { ProjectModule = new ProjectModule() { ProjectId = project } };
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

            return View(new AntigateModulViewModel(config)
            {
                HttpModules = ProjectModuleRegistry.GetModules<HttpProviderRuntimeConfig, HttpProviderData>(_context, _moduleRegistry, project)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(AntigateModulViewModel viewModel)
        {
            var modul = _moduleRegistry.CreateModule<AntigateModule>(_context, AntigateModule.ModuleCode);

            modul.CreateAndSave(viewModel.Config);
            
            return RedirectToAction("Config", "Project", new { Id = viewModel.Project });
        }

        [HttpPost]
        public IActionResult Edit(AntigateModulViewModel viewModel)
        {
            var modul = _moduleRegistry.OpenModule<AntigateModule>(_context,AntigateModule.ModuleCode, viewModel.Id);

            var result = modul.SaveConfig(viewModel.Config);
            if (result)
            {
                TempData["message"] = $" Config of {modul.Name} has been changed";
            }

            return View(new AntigateModulViewModel(modul.Id, modul.Configuration) {
                HttpModules = ProjectModuleRegistry.GetModules<HttpProviderRuntimeConfig, HttpProviderData>(_context, _moduleRegistry, modul.Configuration.ProjectModule.ProjectId)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var modul = _moduleRegistry.OpenModule<AntigateModule>(_context,AntigateModule.ModuleCode, id);
            return View(new AntigateModulViewModel(id, modul.Configuration) {
                HttpModules = ProjectModuleRegistry.GetModules<HttpProviderRuntimeConfig, HttpProviderData>(_context, _moduleRegistry, modul.Configuration.ProjectModule.ProjectId)
            });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var module = _moduleRegistry.OpenModule<AntigateModule>(_context,AntigateModule.ModuleCode, id);
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

            var task = new ProjectModuleTask<string, AntigateTaskResult>(_provider, id, User.GetUserId(), request);

            _runtimeQueue.Add(task);
            return new JsonResult(ModuleRuntimeDetails.CreateModuleExecStateDetails(_runtimeQueue[id]));
        }

        public PartialViewResult GetResult(int modulId)
        {
            var modul = _moduleRegistry.OpenModule<AntigateModule>(_context,AntigateModule.ModuleCode, modulId);

            var result = new ModuleTaskResult<AntigateTaskResult>();
            var task = _runtimeQueue[modulId] as ProjectModuleTask<string, AntigateTaskResult>;
            if (task != null)
            {
                result = task.Result;
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
            var result = new ModuleTaskResult<AntigateTaskResult>();
            var task = _runtimeQueue[id] as ProjectModuleTask<string, AntigateTaskResult>;
            if (task != null)
            {
                result = task.Result;
            }
            return PartialView("_Statistics", new ModuleResultViewModel<AntigateTaskResult>(id,result));
        }
    }
}