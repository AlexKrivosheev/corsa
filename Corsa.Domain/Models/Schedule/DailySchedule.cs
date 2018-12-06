using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Corsa.Domain.Tasks
{
    [Table("DailySchedules")] 
    public class DailySchedule : TaskSchedule
    {
        public EnumScheduleWeek Day { get; set; }

        public DateTime Time { get; set; } = new DateTime(2000,1,1);

        public override DateTime AlarmTime(DateTime dateTime)
        {
            int dayOfWeek = (int)dateTime.DayOfWeek;
            int targetDayOfWeek = (int)Day;

            int day = 0;
            if (targetDayOfWeek > dayOfWeek)
            {
                day = targetDayOfWeek - dayOfWeek;
            } else if (targetDayOfWeek < dayOfWeek)
            {
                day = (7 + targetDayOfWeek) - dayOfWeek;
             }
                
            var targetDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day , Time.Hour, Time.Minute, Time.Millisecond);            
            return targetDateTime.AddDays(day);
         }
    }
}
