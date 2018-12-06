using Corsa.Domain.Tasks;
using System.Collections.Generic;

namespace Corsa.Models.Moduls
{
    public class DailyScheduleModelView
    {
        public DailyScheduleModelView(List<DailySchedule> scheduleTable)
        {
            ScheduleTable = scheduleTable;
        }

        public List<DailySchedule> ScheduleTable { get; set; }
    }
}
