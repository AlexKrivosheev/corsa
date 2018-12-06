using System;
using System.Net;
using System.Net.Mail;
using Corsa.Domain.Models.Actions;
using Corsa.Domain.Moduls.LexicalAnalysis;
using Corsa.Domain.Moduls.RutimeModuls.EmailClient;
using Corsa.Domain.Moduls.SerpAnalysis;
using Corsa.Domain.Processing.Moduls;

namespace Corsa.Domain.Moduls.RutimeModuls.EmailNotif
{
    public class EmailNotifModule : Module, IRuntimeModule<NotifMessage, NotifResult>, IConfigModule<EmailNotifConfig>, IRuntimeModule<ModuleTaskResult<SerpModuleData>, NotifResult>, IRuntimeModule<ModuleTaskResult<LexModuleData>, NotifResult>
    {
        public EmailNotifModule()
        {
          
        }
        public EmailNotifModule(int id)
        {
            this.Id = id;
        }

        public int Id { get; set; }

        public override int Code
        {
            get
            {
                return ModuleCode;
            }
        }

        public static int ModuleCode = 1007;

        public EmailNotifConfig Configuration { get; set; }

        public override string Name
        {
            get { return Configuration?.ProjectModule?.Name; }
        }

        public bool CreateAndSave(EmailNotifConfig config)
        {
            config.ProjectModule.Code = ModuleCode;
            config.ProjectModule.CreatedTime = DateTime.Now;
            config.ProjectModule.ProjectId = config.ProjectModule.ProjectId;

            return Context.Repository.AddModule(config.ProjectModule) & Context.Repository.AddEmailNotifConfig(config);
        }

        public bool Load()
        {
            return LoadConfig();
        }

        public bool Drop()
        {
            return Context.Repository.DropModule(Id);
        }

        public bool LoadConfig()
        {
            Configuration = Context.Repository.GetEmailNotifConfig(Id) ?? new EmailNotifConfig();
            return true;
        }

        public NotifResult Run(NotifMessage config)
        {
            return SendMessage(config.Caption, config.Message);
        }

        public NotifResult Run(ModuleTaskResult<SerpModuleData> config)
        {
            return SendMessage($"Module {1} was completed", config.Details.Message);
        }
        public NotifResult Run(ModuleTaskResult<LexModuleData> config)
        {
            return SendMessage($"Module {1} was completed", config.Details.Message);
        }

        public NotifResult SendMessage(string caption, string message) {

            string[] adress = Configuration.Participants.Split(";");
            var emailMessage = new MailMessage();
            foreach (var item in adress)
            {
                emailMessage.To.Add(new MailAddress(item));
            }

            emailMessage.From = new MailAddress(Configuration.Sender);

            emailMessage.Subject = caption;
            emailMessage.Body = message;
            

            using (var client = new SmtpClient(Configuration.HostName, Configuration.Port.HasValue ? Configuration.Port.Value:25))
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(Configuration.User, Configuration.Password);
                client.EnableSsl = Configuration.EnableSsl;
                client.Send(emailMessage);                
            }

            return new NotifResult();
        }

        public bool SaveConfig(EmailNotifConfig config)
        {
            var projectModule = Context.Repository.GetModule(Id);
            projectModule.Name = config.ProjectModule.Name;
            projectModule.Description = config.ProjectModule.Description;

            Configuration.HostName = config.HostName;
            Configuration.Port = config.Port;
            Configuration.User = config.User;
            Configuration.Password = config.Password;
            Configuration.Participants = config.Participants;
            Configuration.EnableSsl = config.EnableSsl;
            Configuration.Sender = config.Sender;

            return Context.Repository.UpdateModule(projectModule) & Context.Repository.UpdateEmailNotifConfig(Configuration);
        }

        public UserActionDetails GetDetails(int id)
        {
            return Context.Repository.GetActionStatistic(id);
        }


    }
}
