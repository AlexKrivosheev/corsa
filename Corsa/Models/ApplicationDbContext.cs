using Microsoft.EntityFrameworkCore;
using Corsa.Domain.Processing.Serp;
using Corsa.Domain.Models.Actions;
using Corsa.Domain.Models.Projects;
using Corsa.Domain.Processing.Moduls.LexicalAnalysis;
using Corsa.Domain.Moduls.LexicalAnalysis;
using Corsa.Domain.Moduls.SerpAnalysis;
using Corsa.Domain.Logging;
using Corsa.Domain.Models.SystemConfig;
using Corsa.Domain.Moduls.Providers.YandexXml;
using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using Corsa.Domain.Tasks;
using Corsa.Domain.Models.Schedule;
using Corsa.Domain.Moduls.RutimeModuls.EmailClient;
using Corsa.Domain.Models.Account;
using Corsa.Domain.Models.Config;

namespace Corsa.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectModule> ProjectModules { get; set; }

        public DbSet<SystemModule> SystemModules { get; set; }

        public DbSet<ProjectModuleResult> ProjectModuleResults { get; set; }

        public DbSet<LexModuleСonfig> LexModuleConfigs { get; set; }

        public DbSet<LexModuleStats> LexModuleStats { get; set; }

        public DbSet<SerpWebPage> SerpPages { get; set; }
        
        public DbSet<UserActionDetails>  UserActionStatistics{ get; set; }
        
        public DbSet<UserAction> UserActions { get; set; }

        public DbSet<Log> UserLogs { get; set; }

        public DbSet<UserActionSettings> UserActionSettings { get; set; }

        public DbSet<SerpModuleConfig> SerpModuleConfigs { get; set; }

        public DbSet<SerpModuleRequestStats> SerpModuleStats { get; set; }

        public DbSet<YandexXmlConfig> YandexXmlConfigs { get; set; }

        public DbSet<YandexDirectConfig> YandexDirectConfigs { get; set; }

        public DbSet<HttpProviderConfig> HttpProviderConfigs { get; set; }

        public DbSet<AntigateConfig> AntigateConfigs { get; set; }

        public DbSet<OneTimeSchedule> OneTimeSchedules { get; set; }

        public DbSet<DailySchedule> DailySchedules { get; set; }

        public DbSet<ScheduleTask> ScheduleTasks { get; set; }

        public DbSet<EmailNotifConfig> EmailNotifConfigs { get; set; }

        public DbSet<UserSettings> UserSettings { get; set; }

        public DbSet<LinkedModule> LinkedModules { get; set; }
    }
}
