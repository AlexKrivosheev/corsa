using Corsa.Domain.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Corsa.Domain.Models.Schedule
{    
    [Table("OneTimeSchedules")]
    public class OneTimeSchedule : TaskSchedule
    {
        public DateTime DateTime { get; set; } = new DateTime(2000,1,1);

        public override DateTime AlarmTime(DateTime dateTime)
        {
            return DateTime;
        }
    }
}
