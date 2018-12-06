using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Tasks
{
    public class TaskStateDetails
    {
        public TaskState State { get; set; }
        public DateTime StartedTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string Message { get; set; }
    }
}
