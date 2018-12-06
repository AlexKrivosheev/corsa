using Corsa.Domain.Models.Config;
using Corsa.Domain.Models.Requests;
using Corsa.Infrastructure;
using Corsa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace Corsa.Controllers
{
    public class SettingsController : Controller
    {
        private ISourceRepository _reposiotry;

        public SettingsController(ISourceRepository repository)
        {
            _reposiotry = repository;
        }

        public IActionResult Index()
        {
            var userSettings = _reposiotry.GetUserSettings(User.GetUserId());
            var languages = new List<Language>()
            {
                new Language(){Id = "en-US", Name ="English"},
                new Language(){Id = "ru-RU", Name ="Russian"}
            };
            
            return View(new SettingsViewModel(userSettings,languages, TimeZoneInfo.GetSystemTimeZones()));
        }
        
        [HttpPost]        
        public IActionResult Save(SettingsViewModel viewModel)
        {
            viewModel.Settings.UserId = User.GetUserId();

            bool result = false;
            var settings = _reposiotry.GetUserSettings(User.GetUserId());
            if (settings == null)
            {                
                result = _reposiotry.AddUserSettings(viewModel.Settings);
            }
            else
            {                
                result = _reposiotry.UpdateUserSettings(viewModel.Settings);
            }

            if (result)
            {
                TempData["message"] = $" Settings has been changed";
            }

            return RedirectToAction("Index");
        }

    }
}