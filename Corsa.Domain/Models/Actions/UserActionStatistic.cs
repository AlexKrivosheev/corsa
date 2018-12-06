using Corsa.Domain.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corsa.Domain.Models.Actions
{
    [Table("UserActionStats")]
    public class UserActionDetails
    {        
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ActionId { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime FinishedTime { get; set; }
        public ActionExecutionResult Result { get; set; }
        public string Message { get; set; }
        public int? ParentId { get; set; }

        [ForeignKey("UserActionStatsId")]
        public List<Log> Logs { get; set; } = new List<Log>();

        [ForeignKey("ParentId")]
        public UserActionDetails Parent{ get; set; }

        [ForeignKey("ParentId")]
        public List<UserActionDetails> Children { get; set; } = new List<UserActionDetails>();

        public List<Log> GetChildLogs(UserActionDetails details)
        {
            List<Log> result = new List<Log>(details.Logs);

            foreach (var child in details.Children)
            {
                result.AddRange(GetChildLogs(child));
            }

            return result;
        }

        public List<Log> GetFullLogs()
        {
            List<Log> result = new List<Log>(GetChildLogs(this));
            result.Sort((log1, log2) => { return DateTime.Compare(log1.CreatedTime, log2.CreatedTime); });

            return result;
        }

    }
}
