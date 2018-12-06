using Corsa.Domain.Models.Schedule;
using Corsa.Domain.Tasks;
using System.Collections.Generic;

namespace Corsa.Models.Moduls
{
    public class OneTimeScheduleViewModel
    {
        public OneTimeScheduleViewModel(List<OneTimeSchedule> scheduleTable)
        {
            ScheduleTable = scheduleTable;
        }

        public List<OneTimeSchedule> ScheduleTable { get; set; }
    }
}
