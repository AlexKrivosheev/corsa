using Corsa.Domain.Exceptions;
using Corsa.Domain.Models;
using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.Requests;
using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.Providers;
using Corsa.Domain.Moduls.SerpAnalysis;
using Corsa.Domain.Processing;
using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Processing.Serp;
using Corsa.Domain.Tasks.Modules;
using Corsa.Infrastructure;
using Corsa.Infrastructure.Logging;
using Corsa.Infrastructure.Tasks;
using Corsa.Models;
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

namespace Corsa.Controllers
{
    [Authorize]
    public class SerpAnalysisController : Controller
    {
        private ISourceRepository _reposiotry;
        private IConfiguration _configuration;
        private UserManager<IdentityUser> _userManager;
        private ILoggerFactory _loggerFactory;
        private IProjectModuleRegistry _modulRegistry;
        private ILogger _logger;
        private RuntimeTaskService _runtimeQueue;
        private IServiceProvider _provider;
        IStringLocalizer<SerpAnalysisController> _controllerLocalizer;
        IStringLocalizer<Module> _modulelocalizer;
        RuntimeContext _context;

        public SerpAnalysisController(IStringLocalizer<SerpAnalysisController> controllerLocalizer, IStringLocalizer<Module> moduleLocalizer, IServiceProvider provider, ISourceRepository repository, RuntimeTaskService taskManager, IProjectModuleRegistry modulRegistry, IConfiguration configuration,  UserManager<IdentityUser> userManager, ILoggerFactory loggerFactory)
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
            var modul = _modulRegistry.OpenModule<SerpModule>(_context,SerpModule.ModuleCode, id);            
            return View(new SerpAnalysisModuleViewModel(id,modul.Configuration, modul.Results));
        }

        [HttpGet]
        public IActionResult Create(int project)
        {
            SerpModuleConfig config = new SerpModuleConfig() { ProjectModule = new ProjectModule() { ProjectId = project }};
            try
            {
                var targetProject = _reposiotry.GetProject(project);
                if (targetProject == null)
                {
                    throw new UserException($"Project {project} isn't find");
                }
                
                //config.Project = _reposiotry.GetProject(project);
            }
            catch (UserException exc)
            {
                _logger.LogError(exc.Message);
                TempData["message"] = exc.Message;
            }

            return View(new SerpAnalysisModuleViewModel(config) {
                DataProviders = ProjectModuleRegistry.GetModules<string, List<SerpWebPage>>(_context, _modulRegistry, project),
                AvailableLinkedModules = ProjectModuleRegistry.GetModules<ModuleTaskResult<SerpModuleData>>(_context, _modulRegistry, project)
            });
        }

  
        [HttpPost]
        public async Task<IActionResult> Create(SerpAnalysisModuleViewModel viewModel)
        {
            var modul = _modulRegistry.CreateModule<SerpModule>(_context, SerpModule.ModuleCode);

            modul.CreateAndSave(viewModel.Config);            
            return RedirectToAction("Index", "Project", new { Id = viewModel.Project });
        }

        [HttpPost]
        public IActionResult Edit(SerpAnalysisModuleViewModel viewModel) 
        {
            var modul = _modulRegistry.OpenModule<SerpModule>(_context, SerpModule.ModuleCode, viewModel.Id);

            var result = modul.SaveConfig(viewModel.Config);
            if (result)
            {
                TempData["message"] = $" Config of {modul.Name} has been changed";
            }

            modul.LoadConfig();

            return View(new SerpAnalysisModuleViewModel(modul.Id, modul.Configuration) {
                DataProviders = ProjectModuleRegistry.GetModules<string, List<SerpWebPage>>(_context, _modulRegistry, modul.Configuration.ProjectModule.ProjectId),
                AvailableLinkedModules = ProjectModuleRegistry.GetModules<ModuleTaskResult<SerpModuleData>>(_context, _modulRegistry, modul.Configuration.ProjectModule.ProjectId)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var modul = _modulRegistry.OpenModule<SerpModule>(_context, SerpModule.ModuleCode, id);
            return View(new SerpAnalysisModuleViewModel(id, modul.Configuration) {
                DataProviders = ProjectModuleRegistry.GetModules<string, List<SerpWebPage>>(_context, _modulRegistry, modul.Configuration.ProjectModule.ProjectId),
                AvailableLinkedModules = ProjectModuleRegistry.GetModules<ModuleTaskResult<SerpModuleData>>(_context, _modulRegistry, modul.Configuration.ProjectModule.ProjectId)
            });
        }
        
        public IActionResult DropStats(int modulId, int resultId)
        {
            var modul = _modulRegistry.OpenModule<SerpModule>(_context,SerpModule.ModuleCode, modulId);
            modul.DropResult(resultId);
            return RedirectToAction("Index", new { id = modulId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var module = _modulRegistry.OpenModule<SerpModule>(_context,SerpModule.ModuleCode, id);
            module.Drop();            
            return RedirectToAction("Index","Project", new {id = module.Configuration.ProjectModule.ProjectId});
        }

        public JsonResult Update(int id)
        {
            var task = new ProjectModuleTask<SerpModuleData>(_provider,id, User.GetUserId());                        
            _runtimeQueue.Add(task);
            return new JsonResult(ModuleRuntimeDetails.CreateModuleExecStateDetails(_runtimeQueue[id]));
        }

        public JsonResult GetModuleState(int id)
        {
            return new JsonResult(ModuleRuntimeDetails.CreateModuleExecStateDetails(_runtimeQueue[id]));
        }

        public PartialViewResult GetResult(int modulId, int resultId)
        {
            var modul =   _modulRegistry.OpenModule<SerpModule>(_context,SerpModule.ModuleCode, modulId);
            modul.LoadResults();

            var targetResult = modul.GetResult(resultId);

            _runtimeQueue.EraseState(modulId);

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
            var modul = _modulRegistry.OpenModule<SerpModule>(_context,SerpModule.ModuleCode, id);                        
            modul.LoadResults();

            return PartialView("_Statistics", new SerpAnalysisModuleViewModel(id, modul.Configuration, modul.Results));
        }


    }
}