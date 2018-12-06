using Corsa.Domain.Logging;
using Corsa.Domain.Models.Account;
using Corsa.Domain.Models.Actions;
using Corsa.Domain.Models.Config;
using Corsa.Domain.Models.Projects;
using Corsa.Domain.Models.Schedule;
using Corsa.Domain.Moduls.LexicalAnalysis;
using Corsa.Domain.Moduls.Providers.YandexXml;
using Corsa.Domain.Moduls.RutimeModuls.EmailClient;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using Corsa.Domain.Moduls.SerpAnalysis;
using Corsa.Domain.Processing.Moduls.LexicalAnalysis;
using Corsa.Domain.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corsa.Domain.Models.Requests
{
    public interface ISourceRepository
    {
        #region Settings

        bool AddUserSettings(UserSettings settings);

        UserSettings GetUserSettings(int id);

        UserSettings GetUserSettings(string  userId);

        bool UpdateUserSettings(UserSettings settings);

        #endregion

        #region Project

        bool AddProject(Project request);

        bool UpdateProject(Project request);

        Project GetProject(int id);
        
        IQueryable<Project> GetProjects(string owner);

        IQueryable<ProjectModule> GetProjectModules(int projectId);

        IQueryable<ProjectModule> GetProjectModules();

        IQueryable<ProjectModuleResult> GetProjectModuleResults(int moduleId);
        
        bool DropModuleResult(int resultId);

        bool ClearModuleResults(int moduleId);

        bool AddModuleResults(ProjectModuleResult result);

        bool DropProject(int id);

        bool AddModule(ProjectModule modul);

        bool UpdateModule(ProjectModule modul);

        bool DropModule(int id);

        ProjectModule GetModule(int id);

        bool UpdateLinkedModule(LinkedModule module);

        bool AddLinkedModule(LinkedModule module);

        List<LinkedModule> GetLinkedModules(int id);

        LinkedModule GetLinkedModule(int id);

        bool RemoveSchedule(LinkedModule schedule);

        #endregion

        #region Action

        IQueryable<UserAction> UserActions { get; }

        IQueryable<UserActionDetails> ActionStatistics { get; }

        IQueryable<UserActionDetails> GetActionStatistics(string owner);

        UserActionDetails GetActionStatistic(int id);

        bool UpdateActionStatistics(UserActionDetails statistic);

        bool AddActionStatistics(UserActionDetails statistic);

        bool DropActionStatistics(int id);

        IQueryable<UserActionSettings> UserActionsSettings { get; }

        IQueryable<UserActionSettings> GetUserActionsSettings(string owner);

        IQueryable<UserActionSettings> GetUserActionsSettings(int actionId, string owner, DateTime currentTime);

        UserActionSettings GetUserActionsSettings(int id);

        bool UpdateUserActionSettings(UserActionSettings settings);

        #endregion

        #region Lexical Analysis
        
        bool AddLexModuleConfig(LexModuleСonfig config);

        LexModuleСonfig GetLexModuleConfig(int modulId);

        bool DropLexModuleStats(int configId);

        IQueryable<LexModuleStats> GetLexModuleStats(int configId);

        bool AddLexModuleStats(LexModuleStats stats);

        bool UpdateLexModuleConfig(LexModuleСonfig config);

        #endregion

        #region Serp Analysis

        bool AddSerpModuleConfig(SerpModuleConfig config);

        SerpModuleConfig GetSerpModuleConfig(int modulId);

        bool DropSerpModuleStats(int modulId);

        IQueryable<SerpModuleRequestStats> GetSerpModuleRequestStats(int resultId);

        bool AddSerpModuleRequestStats(SerpModuleRequestStats stats);

        bool UpdateSerpModuleRequestStats(SerpModuleRequestStats config);

        bool UpdateSerpModuleConfig(SerpModuleConfig config);

        #endregion

        #region Yandex XML
        
        bool AddYandexXmlConfig(YandexXmlConfig config);

        YandexXmlConfig GetYandexXmlConfig(int modulId);
        
        bool UpdateYandexXmlConfig(YandexXmlConfig config);

        #endregion

        #region Yandex Direct

        bool AddYandexDirectConfig(YandexDirectConfig config);

        YandexDirectConfig GetYandexDirectConfig(int modulId);

        bool UpdateYandexDirectConfig(YandexDirectConfig config);

        #endregion

        #region Http Provider

        bool AddHttpProviderConfig(HttpProviderConfig config);

        HttpProviderConfig GetHttpProviderConfig(int modulId);

        bool UpdateHttpProviderConfig(HttpProviderConfig config);

        #endregion

        #region Antigate

        bool AddAntigateConfig(AntigateConfig config);

        AntigateConfig GetAntigateConfig(int modulId);

        bool UpdateAntigateConfig(AntigateConfig config);

        #endregion

        #region Schedules

        bool RemoveSchedule(DailySchedule schedule);

        bool RemoveSchedule(OneTimeSchedule schedule);

        bool AddSchedule(OneTimeSchedule schedule);

        bool AddSchedule(DailySchedule schedule);

        IQueryable<TaskSchedule> GetSchedules(int moduleId);

        IQueryable<ScheduleTask> ScheduleTasks();

        bool UpdateScheduleTask(ScheduleTask task);

        bool AddScheduleTask(ScheduleTask task);

        ScheduleTask GetScheduleTask(int id);

        #endregion

        #region Email Nofitication

        bool AddEmailNotifConfig(EmailNotifConfig config);

        EmailNotifConfig GetEmailNotifConfig(int modulId);

        bool UpdateEmailNotifConfig(EmailNotifConfig config);

        #endregion
    }
}
