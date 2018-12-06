using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Tasks
{
    public interface ISchedulable
    {
        ICollection<TaskSchedule> GetSchedules();
    }
}
