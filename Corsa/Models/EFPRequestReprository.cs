using Corsa.Domain.Logging;
using Corsa.Domain.Models.Account;
using Corsa.Domain.Models.Actions;
using Corsa.Domain.Models.Config;
using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.Requests;
using Corsa.Domain.Models.Schedule;
using Corsa.Domain.Moduls.LexicalAnalysis;
using Corsa.Domain.Moduls.Providers.YandexXml;
using Corsa.Domain.Moduls.RutimeModuls.EmailClient;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using Corsa.Domain.Moduls.SerpAnalysis;
using Corsa.Domain.Processing.Moduls.LexicalAnalysis;
using Corsa.Domain.Tasks;
using Corsa.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corsa.Domain.Models
{
    public class EFPRequestReprository : ISourceRepository, IDisposable
    {
        private ApplicationDbContext _context;

       
        public EFPRequestReprository(DbContextOptions<ApplicationDbContext> options)
        {
            _context = new ApplicationDbContext(options);            
        }

        public IQueryable<Project> GetProjects(string owner)
        {
            return _context.Projects.Where(project=>string.Equals(project.UserId,owner,StringComparison.InvariantCultureIgnoreCase));
        }

        public Project GetProject(int id)
        {
            return _context.Projects.Where(project => project.Id == id).Include(project => project.Moduls).SingleOrDefault();
        }

        public IQueryable<UserActionDetails> ActionStatistics
        {
            get { return _context.UserActionStatistics; }
        }

        public IQueryable<UserAction> UserActions
        {
            get
            {
                return _context.UserActions;
            }
        }

        public IQueryable<UserActionSettings> UserActionsSettings
        {
            get
            {
                return _context.UserActionSettings;
            }
        }

        public IQueryable<UserActionSettings> GetUserActionsSettings(string owner)
        {
            return UserActionsSettings.Where(settings => string.Equals(settings.UserId, owner, System.StringComparison.InvariantCultureIgnoreCase));
        }

        public IQueryable<UserActionSettings> GetUserActionsSettings(int actionId, string owner, DateTime currentTime)
        {
            return GetUserActionsSettings(owner).Where(settings => settings.ActionId == actionId && settings.PeriodStart <= currentTime && currentTime<= settings.PeriodEnd);
        }

        public UserActionSettings GetUserActionsSettings(int id)
        {
            return UserActionsSettings.Where(settings => settings.Id == id).SingleOrDefault();
        }

        public bool UpdateUserActionSettings(UserActionSettings settings)
        {
            var target = GetUserActionsSettings(settings.Id);
            if (target != null)
            {
                target.PeriodEnd = settings.PeriodEnd;
                target.PeriodStart = settings.PeriodStart;
                target.Priority = settings.Priority;
                target.Quantity = settings.Quantity;
                target.UserId = settings.UserId;                
                return _context.SaveChanges() == 0;
            }

            return _context.SaveChanges() == 0;
        }

        public IQueryable<UserActionDetails> GetActionDetails(string owner)
        {
            return ActionStatistics.Where(statistic => string.Equals(statistic.UserId, owner, System.StringComparison.InvariantCultureIgnoreCase));
        }

        public UserActionDetails GetActionDetails(int id, string owner)
        {
            return GetActionDetails(owner).Where(statistic => statistic.Id == id).SingleOrDefault();
        }

        public bool UpdateActionStatistics(UserActionDetails details)
        {
            var target = _context.UserActionStatistics.SingleOrDefault(item => item.Id == details.Id && item.UserId == details.UserId);
            if (target != null)
            {
                target.CreatedTime  = details.CreatedTime;
                target.FinishedTime = details.FinishedTime;
                target.Message = details.Message;
                target.Result = details.Result;
                target.UserId = details.UserId;
                target.Result = details.Result;
                return _context.SaveChanges() == 0;
            }

            return _context.SaveChanges() == 0;
        }

        public static void EnsurePopulated(IApplicationBuilder app, AppIdentityDbContext identityDbContext, IConfiguration Configuration)
        {

        }
        public bool AddActionStatistics(UserActionDetails details)
        {
            _context.UserActionStatistics.Add(details);
            return _context.SaveChanges() == 0;
        }

        public bool DropActionStatistics(int id)
        {
            var target = _context.UserActionStatistics.SingleOrDefault(statistic => statistic.Id == id);
            if (target != null)
            {
                _context.UserActionStatistics.Remove(target);
                return _context.SaveChanges() == 0;
            }

            return false;
        }

        public bool AddProject(Project project)
        {
            _context.Projects.Add(project);
            return _context.SaveChanges() == 0;
        }
  
        public bool UpdateProject(Project project)
        {
            var target = _context.Projects.SingleOrDefault(item => item.Id == project.Id);
            if (target != null)
            {
                target.Name = project.Name;                
                return _context.SaveChanges() == 0;
            }

            return false;
        }


        public bool DropProject(int id)
        {
            var target = _context.Projects.SingleOrDefault(project => project.Id == id);
            if (target != null)
            {
                _context.Projects.Remove(target);
                return _context.SaveChanges() == 0;

            }

            return false;
        }

        public IQueryable<UserActionDetails> GetActionStatistics(string owner)
        {
            throw new NotImplementedException();
        }

        public UserActionDetails GetActionStatistic(int id)
        {
            return _context.UserActionStatistics.Where(statistics => statistics.Id == id)
                .Include(stats => stats.Children).ThenInclude(child=>child.Logs)
                .Include(stats=>stats.Logs).SingleOrDefault();
        }

        public bool AddModule(ProjectModule modul)
        {
            _context.ProjectModules.Add(modul);
            return _context.SaveChanges() == 0;
        }

        public bool AddLexModuleConfig(LexModuleСonfig config)
        {
            _context.LexModuleConfigs.Add(config);
            return _context.SaveChanges() == 0;
        }

        public bool UpdateLexModuleConfig(LexModuleСonfig config)
        {
            var target = _context.LexModuleConfigs.SingleOrDefault(item => item.Id == config.Id);
            if (target != null)
            {
                target.Text = config.Text;
                target.RequestAttempt = config.RequestAttempt;
                target.HttpModuleId = config.HttpModuleId;
                target.ProjectModuleId = config.ProjectModuleId;                
                target.DataProviderId = config.DataProviderId;
                return _context.SaveChanges() == 0;
            }

            return false;
        }


        public LexModuleСonfig GetLexModuleConfig(int modulId)
        {
            return _context.LexModuleConfigs.Where(requestConfig => requestConfig.ProjectModuleId == modulId)
                .Include(conf => conf.DataProvider)
                .Include(conf => conf.ProjectModule).ThenInclude(module=> module.Project)                         
                .Include(conf => conf.ProjectModule).ThenInclude(module => module.LinkedModules)
                .Include(conf => conf.ProjectModule).ThenInclude(module => module.OneTimeSchedules)
                .Include(conf => conf.ProjectModule).ThenInclude(module => module.DailySchedules)
                .SingleOrDefault();
        }

        public IQueryable<LexModuleStats> GetLexModuleStats(int configId)
        {
            return _context.LexModuleStats.Where(stats => stats.LexModuleConfigId == configId)
                        .Include(stats => stats.FrequencyData)
                        .Include(stats => stats.SerpPages)
                        .Include(stats => stats.Words);
        }

        public bool DropModule(int id)
        {
            var target = _context.ProjectModules.SingleOrDefault(modul => modul.Id == id);
            if (target != null)
            {
                _context.ProjectModules.Remove(target);
                return _context.SaveChanges() == 0;
            }
            return false;
        }

        public bool UpdateModule(ProjectModule modul)
        {
            var target = _context.ProjectModules.SingleOrDefault(item => item.Id == modul.Id);
            if (target != null)
            {
                target.Name = modul.Name;
                target.Description = modul.Description;
                return _context.SaveChanges() == 0;
            }

            return false;
        }

        public ProjectModule GetModule(int id)
        {


            return _context.ProjectModules.Include(project=>project.Project).Where(target => target.Id == id).SingleOrDefault();
        }

        public bool DropLexModuleStats(int configId)
        {
            _context.LexModuleStats.RemoveRange(_context.LexModuleStats.Where(stats => stats.LexModuleConfigId == configId));
            return _context.SaveChanges() == 0;
        }

        public bool AddLexModuleStats(LexModuleStats stats)
        {
             _context.LexModuleStats.Add(stats);
            return _context.SaveChanges() == 0;
        }

        public bool AddSerpModuleConfig(SerpModuleConfig config)
        {
            _context.SerpModuleConfigs.Add(config);
            return _context.SaveChanges() == 0;
        }

        public SerpModuleConfig GetSerpModuleConfig(int modulId)
        {
            return _context.SerpModuleConfigs.Where(config => config.ProjectModuleId == modulId)
                .Include(conf => conf.DataProvider)
                .Include(conf => conf.ProjectModule).ThenInclude(module => module.Project)
                .Include(conf => conf.ProjectModule).ThenInclude(module => module.LinkedModules)
                .Include(conf => conf.ProjectModule).ThenInclude(module => module.OneTimeSchedules)
                .Include(conf => conf.ProjectModule).ThenInclude(module => module.DailySchedules)
                .Include(conf => conf.Requests)                
                .SingleOrDefault();
        }

        public bool DropSerpModuleStats(int modulId)
        {
            _context.SerpModuleStats.RemoveRange(_context.SerpModuleStats.Where(stats => stats.ModuleResult.ProjectModuleId == modulId));
            return _context.SaveChanges() == 0;
        }

        public IQueryable<SerpModuleRequestStats> GetSerpModuleRequestStats(int resultId)
        {
            return _context.SerpModuleStats.Where(target => target.ProjectModuleResultId== resultId);
        }

        public bool AddSerpModuleRequestStats(SerpModuleRequestStats stats)
        {
             _context.SerpModuleStats.Add(stats);
            return _context.SaveChanges() == 0;
        }

        public bool AddSerpModuleRequestStats(List<SerpModuleRequestStats> stats)
        {
            _context.SerpModuleStats.AddRange(stats);
            return _context.SaveChanges() == 0;
        }

        public bool UpdateSerpModuleRequestStats(SerpModuleRequestStats stats)
        {
            var target = _context.SerpModuleStats.SingleOrDefault(serpStats => serpStats.Id == stats.Id);
            if (target != null)
            {
                target.Position = stats.Position;
                target.DetectionTime = stats.DetectionTime;                
                return _context.SaveChanges() == 0;
            }

            return false;
        }

        public IQueryable<ProjectModuleResult> GetProjectModuleResults(int moduleId)
        {
            return _context.ProjectModuleResults.Where(moduleResult => moduleResult.ProjectModuleId == moduleId)
                .Include(result => result.Stats).ThenInclude(stats => stats.Logs)
                .Include(result => result.Stats).ThenInclude(stats => stats.Children).ThenInclude(child => child.Logs)
                .Include(result => result.Stats).ThenInclude(stats => stats.Children).ThenInclude(child => child.Children)
                .Include(result => result.Stats).ThenInclude(stats => stats.Children).ThenInclude(child => child.Children).ThenInclude(child => child.Logs)
                .Include(result => result.Stats).ThenInclude(stats => stats.Children).ThenInclude(child => child.Children).ThenInclude(child => child.Children).ThenInclude(child => child.Logs);
        }

        public bool UpdateSerpModuleConfig(SerpModuleConfig config)
        {
            var target = _context.SerpModuleConfigs.SingleOrDefault(serpConfig => serpConfig.Id == config.Id);
            if (target != null)
            {                                
                target.Url = config.Url;
                target.RequestAttempt = config.RequestAttempt;
                target.DataProviderId = config.DataProviderId;
                target.Requests = config.Requests;
                target.OneTimeSchedules = config.OneTimeSchedules;
                target.DailySchedules = config.DailySchedules;
                return _context.SaveChanges() == 0;
            }

            return false;
        }

        public bool AddModuleResults(ProjectModuleResult result)
        {
            _context.ProjectModuleResults.Add(result);
            return _context.SaveChanges() == 0;
        }

        public bool DropModuleResult(int resultId)
        {
            _context.ProjectModuleResults.RemoveRange(_context.ProjectModuleResults.Where(stats => stats.Id == resultId));
            return _context.SaveChanges() == 0;
        }

        public bool ClearModuleResults(int moduleId)
        {
            _context.ProjectModuleResults.RemoveRange(_context.ProjectModuleResults.Where(result => result.ProjectModuleId == moduleId));
            return _context.SaveChanges() == 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public bool AddYandexXmlConfig(YandexXmlConfig config)
        {
            _context.YandexXmlConfigs.Add(config);
            return _context.SaveChanges() == 0;
        }

        public YandexXmlConfig GetYandexXmlConfig(int modulId)
        {
            return _context.YandexXmlConfigs.Where(config => config.ProjectModuleId == modulId).Include(module=>module.ProjectModule).ThenInclude(module => module.Project).SingleOrDefault();
        }

        public bool UpdateYandexXmlConfig(YandexXmlConfig config)
        {
            var target = _context.YandexXmlConfigs.SingleOrDefault(item => item.Id == config.Id);
            if (target != null)
            {
                target.User = config.User;
                target.Key = config.Key;
                target.Region = config.Region;                
                target.Filter = config.Filter;
                target.PageLimit = config.PageLimit;
                target.ProjectModuleId = config.ProjectModuleId;                
                return _context.SaveChanges() == 0;
            }

            return false;
        }

        public bool AddYandexDirectConfig(YandexDirectConfig config)
        {
            _context.YandexDirectConfigs.Add(config);
            return _context.SaveChanges() == 0;
        }

        public YandexDirectConfig GetYandexDirectConfig(int modulId)
        {
            return _context.YandexDirectConfigs.Where(config => config.ProjectModuleId == modulId).Include(module => module.ProjectModule).ThenInclude(module => module.Project).SingleOrDefault();
        }

        public bool UpdateYandexDirectConfig(YandexDirectConfig config)
        {
            var target = _context.YandexDirectConfigs.SingleOrDefault(item => item.Id == config.Id);
            if (target != null)
            {                
                target.Region = config.Region;
                target.PageLimit = config.PageLimit;                
                target.AntigateId = config.AntigateId;
                target.HttpModuleId = config.HttpModuleId;
                return _context.SaveChanges() == 0;
            }

            return false;
        }

        public bool AddHttpProviderConfig(HttpProviderConfig config)
        {
            _context.HttpProviderConfigs.Add(config);
            return _context.SaveChanges() == 0;
        }

        public HttpProviderConfig GetHttpProviderConfig(int modulId)
        {
            return _context.HttpProviderConfigs.Where(config => config.ProjectModuleId == modulId).Include(module => module.ProjectModule).ThenInclude(module => module.Project).SingleOrDefault();
        }

        public bool UpdateHttpProviderConfig(HttpProviderConfig config)
        {
            var target = _context.HttpProviderConfigs.SingleOrDefault(item => item.Id == config.Id);
            if (target != null)
            {
                target.ProxyHost = config.ProxyHost;
                target.ProxyPort = config.ProxyPort;
                target.ProxyUser = config.ProxyUser;
                target.ProxyPassword = config.ProxyPassword;

                return _context.SaveChanges() == 0;
            }

            return false;
        }

        public IQueryable<ProjectModule> GetProjectModules(int projectId)
        {
            return _context.ProjectModules.Include(project => project.Project).Where(target => target.ProjectId == projectId);
        }

        public IQueryable<ProjectModule> GetProjectModules()
        {
            return _context.ProjectModules.Include(m => m.Project)
                                                .Include(m => m.LinkedModules)
                                                .Include(m=>m.DailySchedules)
                                                .Include(m => m.OneTimeSchedules);
        }

        public bool AddAntigateConfig(AntigateConfig config)
        {
            _context.AntigateConfigs.Add(config);
            return _context.SaveChanges() == 0;
        }

        public AntigateConfig GetAntigateConfig(int modulId)
        {
            return _context.AntigateConfigs.Where(config => config.ProjectModuleId == modulId).Include(module => module.ProjectModule).ThenInclude(module => module.Project).SingleOrDefault();
        }

        public bool UpdateAntigateConfig(AntigateConfig config)
        {
            var target = GetAntigateConfig(config.ProjectModuleId);
            if (target != null)
            {
                target.Id = config.Id;
                target.LanguagePool = config.LanguagePool;
                target.SoftId = config.SoftId;
                target.CallbackUrl = config.CallbackUrl;
                target.HttpModuleId = config.HttpModuleId;
                return _context.SaveChanges() == 0;
            }

            return _context.SaveChanges() == 0;            
        }

        public bool RemoveSchedule(OneTimeSchedule schedule)
        {
            _context.OneTimeSchedules.Remove(schedule);
            return _context.SaveChanges() == 0;
        }

        public bool RemoveSchedule(DailySchedule schedule)
        {
            _context.DailySchedules.Remove(schedule);
            return _context.SaveChanges() == 0;
        }

        public bool AddSchedule(OneTimeSchedule schedule)
        {
            _context.OneTimeSchedules.Add(schedule);
            return _context.SaveChanges() == 0;
        }

        public bool AddSchedule(DailySchedule schedule)
        {
            _context.DailySchedules.Add(schedule);
            return _context.SaveChanges() == 0;
        }

        public IQueryable<TaskSchedule> GetSchedules(int moduleId)
        {
            List<TaskSchedule> result = new List<TaskSchedule>();
            result.AddRange(_context.DailySchedules.Where(schedule => schedule.ProjectModuleId == moduleId));
            result.AddRange(_context.OneTimeSchedules.Where(schedule => schedule.ProjectModuleId == moduleId));
            return result.AsQueryable();
        }

        public IQueryable<ScheduleTask> ScheduleTasks()
        {
            return _context.ScheduleTasks.Include(task=>task.Module).ThenInclude(module=>module.Project);
        }

        public bool UpdateScheduleTask(ScheduleTask task)
        {
            var target = GetScheduleTask(task.Id);
            if (target != null)
            {
                target.Id = task.Id;
                target.DateTime  = task.DateTime;
                target.State = task.State;                                
                return _context.SaveChanges() == 0;
            }

            return _context.SaveChanges() == 0;
        }

        public bool AddScheduleTask(ScheduleTask task)
        {
            _context.ScheduleTasks.Add(task);
            return _context.SaveChanges() == 0;
        }

        public ScheduleTask GetScheduleTask(int id)
        {
            return _context.ScheduleTasks.SingleOrDefault(target=>target.Id==id);
        }

        public bool AddEmailNotifConfig(EmailNotifConfig config)
        {
            _context.EmailNotifConfigs.Add(config);
            return _context.SaveChanges() == 0;
        }

        public EmailNotifConfig GetEmailNotifConfig(int modulId)
        {
            return _context.EmailNotifConfigs.Where(config => config.ProjectModuleId == modulId).Include(module => module.ProjectModule).ThenInclude(module => module.Project).SingleOrDefault();
        }

        public bool UpdateEmailNotifConfig(EmailNotifConfig config)
        {
            var target = GetEmailNotifConfig(config.ProjectModuleId);
            if (target != null)
            {                
                target.HostName = config.HostName;
                target.Port = config.Port;
                target.User = config.User;
                target.Password = config.Password;
                target.Participants = config.Participants;
                target.EnableSsl = config.EnableSsl;
                target.Sender = config.Sender;
                return _context.SaveChanges() == 0;
            }

            return _context.SaveChanges() == 0;
        }

        public UserSettings GetUserSettings(string userId)
        {
            return _context.UserSettings.SingleOrDefault(item => string.Equals(item.UserId, userId, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool UpdateUserSettings(UserSettings settings)
        {
            var target = GetUserSettings(settings.UserId);
            if (target != null)
            {
                target.LanguageId = settings.LanguageId;
                target.TimeZoneId = settings.TimeZoneId;

                return _context.SaveChanges() == 0;
            }
            return false;
        }

        public UserSettings GetUserSettings(int id)
        {
            return _context.UserSettings.SingleOrDefault(item => item.Id==id);
        }

        public bool UpdateLinkedModule(LinkedModule module)
        {
            var target = GetLinkedModule(module.Id);
            if (target != null)
            {
                target.ProjectModuleId = module.ProjectModuleId;
                target.Description = module.Description;

                return _context.SaveChanges() == 0;
            }
            return false;
        }

        public bool AddLinkedModule(LinkedModule module)
        {
            _context.LinkedModules.Add(module);
            return _context.SaveChanges() == 0;
        }

        public LinkedModule GetLinkedModule(int id)
        {
            return _context.LinkedModules.SingleOrDefault(item => item.Id == id);
        }

        public bool RemoveSchedule(LinkedModule schedule)
        {
            _context.LinkedModules.Remove(schedule);
            return _context.SaveChanges() == 0;
        }

        public List<LinkedModule> GetLinkedModules(int id)
        {
            return _context.LinkedModules.Where(item => item.ProjectModuleId == id)
                .Include(module=>module.LinkedProjectModule).ToList();
        }

        public bool AddUserSettings(UserSettings settings)
        {
            _context.UserSettings.Add(settings);
            return _context.SaveChanges() == 0;
        }
    }
}

