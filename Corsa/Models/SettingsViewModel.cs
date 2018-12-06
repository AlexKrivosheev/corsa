using Corsa.Domain.Models.Account;
using Corsa.Domain.Models.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Corsa.Models
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {

        }

        public SettingsViewModel(UserSettings settings, List<Language> languages, ReadOnlyCollection<TimeZoneInfo> zones)
        {
            this.Settings = settings;
            this.Languages = languages;
            this.Zones = zones;
        }

        public UserSettings Settings { get; set; } = new UserSettings();
      
        public string Language
        {
            get { return Settings.LanguageId; }
            set { Settings.LanguageId = value; }
        }

        public string TimeZone
        {
            get { return Settings.TimeZoneId; }
            set { Settings.TimeZoneId = value; }
        }

        public List<Language> Languages { get; set; }

        public ReadOnlyCollection<TimeZoneInfo> Zones { get; set; }
        

    }
}
