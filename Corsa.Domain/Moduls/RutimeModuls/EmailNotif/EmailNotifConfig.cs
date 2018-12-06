using Corsa.Domain.Moduls.RutimeModuls.HttpProvider;
using Corsa.Domain.Processing.Moduls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Moduls.RutimeModuls.EmailClient
{
    public class EmailNotifConfig : ModuleСonfig
    {
        public string HostName { get; set; }

        public int? Port { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string Sender { get; set; }

        public bool EnableSsl { get; set; }

        public string Participants { get; set; }
    }
}
