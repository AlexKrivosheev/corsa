using Corsa.Domain.Models.Schedule;
using Corsa.Domain.Tasks;
using System.Collections.Generic;

namespace Corsa.Models.Moduls
{
    public class ScheduleViewModel
    {
        public ScheduleViewModel(List<OneTimeSchedule> scheduleTable, List<DailySchedule> dailyTable)
        {
            OneTimeSchedules = scheduleTable;
            DailySchedules = dailyTable;
        }

        public List<OneTimeSchedule> OneTimeSchedules { get; set; }

        public List<DailySchedule> DailySchedules { get; set; }
    }
}
