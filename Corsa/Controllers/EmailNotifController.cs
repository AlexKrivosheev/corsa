using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corsa.Domain.Exceptions;
using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.Requests;
using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.RutimeModuls.EmailClient;
using Corsa.Domain.Moduls.RutimeModuls.EmailNotif;
using Corsa.Domain.Processing;
using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Tasks.Modules;
using Corsa.Infrastructure;
using Corsa.Infrastructure.Logging;
using Corsa.Infrastructure.Tasks;
using Corsa.Models.Moduls;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Corsa.Controllers
{
    public class EmailNotifController : Controller
    {
        private ISourceRepository _reposiotry;
        private IConfiguration _configuration;
        private UserManager<IdentityUser> _userManager;
        private ILoggerFactory _loggerFactory;
        private IProjectModuleRegistry _moduleRegistry;
        private ILogger _logger;
        private RuntimeTaskService _runtimeQueue;
        private IServiceProvider _provider;
        IStringLocalizer<EmailNotifController> _controllerLocalizer;
        IStringLocalizer<Module> _modulelocalizer;
        RuntimeContext _context;

        public EmailNotifController(IStringLocalizer<EmailNotifController> controllerLocalizer, IStringLocalizer<Module> moduleLocalizer, IServiceProvider provider, ISourceRepository repository, RuntimeTaskService taskManager, IProjectModuleRegistry modulRegistry, IConfiguration configuration, UserManager<IdentityUser> userManager, ILoggerFactory loggerFactory)
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
            var modul = _moduleRegistry.OpenModule<EmailNotifModule>(_context, EmailNotifModule.ModuleCode, id);        

            return View(new EmailNotifViewModel(id, modul.Configuration, new List<ModuleTaskResult<NotifResult>>()));
        }

        [HttpGet]
        public IActionResult Create(int project)
        {
            EmailNotifConfig config = new EmailNotifConfig() { ProjectModule = new ProjectModule() { ProjectId = project } };
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

            return View(new EmailNotifViewModel(config));
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmailNotifViewModel viewModel)
        {
            var modul = _moduleRegistry.CreateModule<EmailNotifModule>(_context, EmailNotifModule.ModuleCode);

            modul.CreateAndSave(viewModel.Config);
            
            return RedirectToAction("Config", "Project", new { Id = viewModel.Project });
        }

        [HttpPost]
        public IActionResult Edit(EmailNotifViewModel viewModel)
        {
            var module = _moduleRegistry.OpenModule<EmailNotifModule>(_context, EmailNotifModule.ModuleCode, viewModel.Id);

            var result = module.SaveConfig(viewModel.Config);
            if (result)
            {
                TempData["message"] = $" Config of {module.Name} has been changed";
            }

            return View(new EmailNotifViewModel(module.Id, module.Configuration));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var modul = _moduleRegistry.OpenModule<EmailNotifModule>(_context, EmailNotifModule.ModuleCode, id);
            return View(new EmailNotifViewModel(id, modul.Configuration));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var module = _moduleRegistry.OpenModule<EmailNotifModule>(_context, EmailNotifModule.ModuleCode, id);
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

            var task = new ProjectModuleTask<NotifMessage, NotifResult>(_provider, id, User.GetUserId(), new NotifMessage() { Message = request });

            _runtimeQueue.Add(task);
            return new JsonResult(ModuleRuntimeDetails.CreateModuleExecStateDetails(_runtimeQueue[id]));
        }

        public PartialViewResult GetResult(int modulId)
        {
            var modul = _moduleRegistry.OpenModule<EmailNotifModule>(_context, EmailNotifModule.ModuleCode, modulId);

            var result = new ModuleTaskResult<NotifResult>();
            var task = _runtimeQueue[modulId] as ProjectModuleTask<NotifMessage, NotifResult>;
            if (task != null)
            {
                result = task.Result;
                result.Details = modul.GetDetails(task.Result.Details.Id);                
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
            var result = new ModuleTaskResult<NotifResult>();
            var task = _runtimeQueue[id] as ProjectModuleTask<NotifMessage, NotifResult>;
            if (task != null)
            {
                var modul = _moduleRegistry.OpenModule<EmailNotifModule>(_context, EmailNotifModule.ModuleCode, task.ModuleId);
                result = task.Result;
                result.Details = modul.GetDetails(task.Result.Details.Id);
                
            }
            return PartialView("_Statistics", new ModuleResultViewModel<NotifResult>(id,result));
        }
    }
}