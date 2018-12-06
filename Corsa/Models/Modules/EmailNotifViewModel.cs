using Corsa.Domain.Moduls;
using Corsa.Domain.Moduls.RutimeModuls.EmailClient;
using Corsa.Domain.Moduls.RutimeModuls.EmailNotif;
using System.Collections.Generic;

namespace Corsa.Models.Moduls
{
    public class EmailNotifViewModel : ModuleViewModel<EmailNotifConfig, NotifResult>
    {
        
        public EmailNotifViewModel(int id, EmailNotifConfig config, List<ModuleTaskResult<NotifResult>> results) : base(id, config, results)
        {

        }

        public EmailNotifViewModel(int id, EmailNotifConfig config) : base(id,config)
        {

        }

        public EmailNotifViewModel(EmailNotifConfig config):base(config)
        {

        }

        public EmailNotifViewModel() : base(new EmailNotifConfig())
        {

        }

        public int Id
        {
            get
            {
                return Config.ProjectModule.Id;
            }
            set
            {
                Config.ProjectModule.Id = value;
            }
        }

        public int Project
        {
            get
            {
                return Config.ProjectModule.ProjectId;
            }
            set
            {
                Config.ProjectModule.ProjectId = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return Config.ProjectModule.Project?.Name;
            }
        }
 
        public string Name
        {
            get
            {
                return Config.ProjectModule?.Name;
            }
            set
            {
                Config.ProjectModule.Name = value;
            }
        }

        public string Description
        {
            get
            {
                return Config.ProjectModule.Description;
            }
            set
            {
                Config.ProjectModule.Description = value;
            }
        }

        public string HostName
        {
            get
            {
                return Config.HostName;
            }
            set
            {
                Config.HostName = value;
            }
        }

        public int? Port
        {
            get
            {
                return Config.Port;
            }
            set
            {
                Config.Port = value;
            }
        }

        public bool EnableSsl
        {
            get
            {
                return Config.EnableSsl;
            }
            set
            {
                Config.EnableSsl = value;
            }
        }


        public string Sender
        {
            get
            {
                return Config.Sender;
            }
            set
            {
                Config.Sender = value;
            }
        }

        public string User
        {
            get
            {
                return Config.User;
            }
            set
            {
                Config.User = value;
            }
        }

        public string Password
        {
            get
            {
                return Config.Password;
            }
            set
            {
                Config.Password = value;
            }
        }

        public string Participants
        {
            get
            {
                return Config.Participants;
            }
            set
            {
                Config.Participants = value;
            }
        }

    }
}
