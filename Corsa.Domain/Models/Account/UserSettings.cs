using Corsa.Domain.Models.Config;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Models.Account
{
    public class UserSettings
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string TimeZoneId { get; set; }

        public string LanguageId { get; set; }
    }
}
