using System.Collections.Generic;
using System.Threading.Tasks;
using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.Requests;
using Corsa.Domain.Moduls;
using Corsa.Domain.Processing;
using Corsa.Domain.Processing.Moduls;
using Corsa.Infrastructure;
using Corsa.Models;
using Corsa.Models.Config;
using Corsa.Models.ProjectModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Corsa.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private ISourceRepository _reposiotry;
        private IConfiguration _configuration;
        private UserManager<IdentityUser> _userManager;
        private ILoggerFactory _loggerFactory;
        private ProjectModuleViewRegistry _modulVIewDescriptors;
        RuntimeContext _context;

        public ProjectController(IServiceProvider provide, IStringLocalizer<ProjectController> controllerLocalizer, ISourceRepository repository, IConfiguration configuration, UserManager<IdentityUser> userManager, ILoggerFactory loggerFactory, ProjectModuleViewRegistry modulVIewDescriptors)
        {
            _loggerFactory = loggerFactory;
            _reposiotry = repository;
            _configuration = configuration;
            _userManager = userManager;
            _modulVIewDescriptors = modulVIewDescriptors;
            _context = new RuntimeContext(provide, _reposiotry, null, null, User.GetUserId());
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id.HasValue)
            {
                return View(new ProjectViewModel(_reposiotry.GetProject(id.Value), _modulVIewDescriptors));

            }
            else
            {
                return View("Projects", _reposiotry.GetProjects(User.GetUserId()));
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ProjectViewModel(null, null));
        }

        [HttpPost]
        public IActionResult Edit(Project project)
        {
            _reposiotry.UpdateProject(project);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var project = _reposiotry.GetProject(id);
            return View(new ProjectViewModel(project, _modulVIewDescriptors));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            project.UserId = User.GetUserId();
            _reposiotry.AddProject(project);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            _reposiotry.DropProject(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteModule(int id)
        {
            var target = _reposiotry.GetModule(id);
            if (target != null)
            {
                _reposiotry.DropModule(id);
            }

            return RedirectToAction("Index", new { id = target.ProjectId });
        }

        public IActionResult Config(int id)
        {
            ConfigViewModel model = new ConfigViewModel(_reposiotry.GetProject(id), _modulVIewDescriptors);
            return View(model);
        }


    }
}