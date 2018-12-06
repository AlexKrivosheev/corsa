using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Logging
{
    [Table("UserLogs")]
    public class Log
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ActionId { get; set; }

        public string LogLevel { get; set; }

        public string Message { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? UserActionStatsId { get; set; }
    }
}
