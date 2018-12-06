using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Tasks
{
    public interface ITaskPipline
    {
        event EventHandler<TaskEventArgs> TaskStarted;

        event EventHandler<TaskEventArgs> TaskCompleted;

        event EventHandler<TaskEventArgs> TaskAdded;

        event EventHandler<EventArgs> Stoped;

        event EventHandler<EventArgs> Runned;

        bool Add(ITask task);
                
        bool Run();

        bool Stop();

        bool IsRunned { get; }

    }
   
}
