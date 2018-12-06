using System;


namespace Corsa.Domain.Tasks
{
    public class TaskEventArgs : EventArgs
    {
        public ITask Task { get; set; }

        public TaskEventArgs(ITask task)
        {
            this.Task = task;
        }
    }
}
