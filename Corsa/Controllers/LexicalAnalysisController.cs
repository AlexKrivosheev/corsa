using Corsa.Domain.Exceptions;
using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.Requests;
using Corsa.Domain.Moduls.LexicalAnalysis;
using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Processing.Moduls.LexicalAnalysis;
using Corsa.Domain.Processing.Serp;
using Corsa.Domain.Tasks;
using Corsa.Infrastructure;
using Corsa.Infrastructure.Logging;
using Corsa.Models.Moduls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corsa.Domain.Processing;
using Corsa.Domain.Tasks.Modules;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using Corsa.Infrastructure.Tasks;
using Corsa.Models;
using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.SerpAnalysis;

namespace Corsa.Controllers
{
    [Authorize]
    public class LexicalAnalysisController : Controller
    {
        private ISourceRepository _reposiotry;
        private IConfiguration _configuration;
        private UserManager<IdentityUser> _userManager;
        private ILoggerFactory _loggerFactory;
        private IProjectModuleRegistry _modulRegistry;
        private ILogger _logger;
        private RuntimeTaskService _runtimeQueue;
        private System.IServiceProvider _provider;
        private IStringLocalizer<LexicalAnalysisController> _controllerLocalizer;
        private IStringLocalizer<Module> _moduleLocalizer;
        RuntimeContext _context;

        public LexicalAnalysisController(IStringLocalizer<LexicalAnalysisController> controllerLocalizer, IStringLocalizer<Module> moduleLocalizer, IServiceProvider provider, RuntimeTaskService taskManager, ISourceRepository repository, IProjectModuleRegistry modulRegistry, IConfiguration configuration,  UserManager<IdentityUser> userManager, ILoggerFactory loggerFactory)
        {
            _controllerLocalizer = controllerLocalizer;
            _moduleLocalizer = moduleLocalizer;
            _runtimeQueue = taskManager;
            _provider = provider;
            _loggerFactory = loggerFactory;
            _reposiotry = repository;
            _configuration = configuration;
            _userManager = userManager;
            _modulRegistry = modulRegistry;
            _logger = _loggerFactory.CreateLogger(AppLogger.UserCategory);
            _context = new RuntimeContext(_provider, _reposiotry, null, null, User.GetUserId());
        }

        public async Task<IActionResult> Index(int id)
        {
            _runtimeQueue.EraseState(id);
            var module = _modulRegistry.OpenModule<LexModule>(_context,LexModule.ModuleCode, id); 
            return View(new LexicalAnalysisModuleViewModel(id,module.Configuration, module.Results));
        }

        [HttpGet]
        public IActionResult Create(int project)
        {
            LexModuleСonfig config = new LexModuleСonfig() { ProjectModule = new ProjectModule() { ProjectId = project }};
            try
            {
                var targetProject = _reposiotry.GetProject(project);
                if (targetProject == null)
                {
                    throw new UserException($"Project {project} isn't find");
                }                
            }
            catch (UserException exc)
            {
                _logger.LogError(exc.Message);
                TempData["message"] = exc.Message;
            }

            return View(new LexicalAnalysisModuleViewModel(config) {
                DataProviders = ProjectModuleRegistry.GetModules<string, List<SerpWebPage>>(_context, _modulRegistry, project),
                HttpModules = ProjectModuleRegistry.GetModules<HttpProviderRuntimeConfig, HttpProviderData>(_context, _modulRegistry, project),
                AvailableLinkedModules = ProjectModuleRegistry.GetModules<ModuleTaskResult<LexModuleData>>(_context, _modulRegistry, project)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(LexicalAnalysisModuleViewModel viewModel)
        {
            var modul = _modulRegistry.CreateModule<LexModule>(_context,LexModule.ModuleCode);            
            modul.CreateAndSave(viewModel.Config);
            return RedirectToAction("Index", "Project", new { Id = viewModel.Project });
        }

        [HttpPost]
        public IActionResult Edit(LexicalAnalysisModuleViewModel viewModel) 
        {
            var module = _modulRegistry.OpenModule<LexModule>(_context, LexModule.ModuleCode, viewModel.Id);
            var result = module.SaveConfig(viewModel.Config);

            if (result)
            {
                TempData["message"] = $" Config of {module.Name} has been changed";
            }

            module.LoadConfig();
            
            return View(new LexicalAnalysisModuleViewModel(module.Id, module.Configuration) {
                DataProviders = ProjectModuleRegistry.GetModules<string, List<SerpWebPage>>(_context, _modulRegistry, module.Configuration.ProjectModule.ProjectId),
                HttpModules = ProjectModuleRegistry.GetModules<HttpProviderRuntimeConfig, HttpProviderData>(_context, _modulRegistry, module.Configuration.ProjectModule.ProjectId),
                AvailableLinkedModules = ProjectModuleRegistry.GetModules<ModuleTaskResult<LexModuleData>>(_context, _modulRegistry, module.Configuration.ProjectModule.ProjectId)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var module = _modulRegistry.OpenModule<LexModule>( _context, LexModule.ModuleCode, id);
            return View(new LexicalAnalysisModuleViewModel(id,module.Configuration) {
                DataProviders = ProjectModuleRegistry.GetModules<string, List<SerpWebPage>>( _context, _modulRegistry, module.Configuration.ProjectModule.ProjectId),
                HttpModules = ProjectModuleRegistry.GetModules<HttpProviderRuntimeConfig, HttpProviderData>(_context, _modulRegistry, module.Configuration.ProjectModule.ProjectId),
                AvailableLinkedModules = ProjectModuleRegistry.GetModules<ModuleTaskResult<LexModuleData>>(_context, _modulRegistry, module.Configuration.ProjectModule.ProjectId)
            });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var module = _modulRegistry.OpenModule<LexModule>(_context, LexModule.ModuleCode, id);
            module.Drop();
            return RedirectToAction("Index", "Project", new { id = module.Configuration.ProjectModule?.ProjectId });
        }
  
        public JsonResult Update(int id)
        {            
            var task = new ProjectModuleTask<LexModuleData>(_provider,id, User.GetUserId());
            _runtimeQueue.Add(task);

            return new JsonResult(ModuleRuntimeDetails.CreateModuleExecStateDetails(_runtimeQueue[id]));
        }

        public JsonResult GetModuleState(int id)
        {
            return new JsonResult(ModuleRuntimeDetails.CreateModuleExecStateDetails(_runtimeQueue[id]));
        }

        public PartialViewResult GetResult(int modulId, int resultId)
        {
            var modul = _modulRegistry.OpenModule<LexModule>(_context,LexModule.ModuleCode, modulId);
            
            var targetResult = modul.GetResult(resultId);

            if (targetResult != null)
            {
                return PartialView("_ModuleResultDetails", targetResult.Details);
            }
            else
            {
                return PartialView("_EmptyModuleResult");
            }
        }

        public PartialViewResult GetStatistics(int id)
        {
            var modul = _modulRegistry.OpenModule<LexModule>(_context, LexModule.ModuleCode, id);
            modul.LoadResults();

            return PartialView("_Statistics", new LexicalAnalysisModuleViewModel(id, modul.Configuration, modul.Results));
        }

    }
}