using Corsa.Domain.Tasks;
using System;

namespace Corsa.Models.Moduls
{
    public enum ModuleRuntmieState
    {
        Absent,
        Wait,
        Running,
        Completed
    }

    public class ModuleRuntimeDetails
    {
        public string Message { get; set; }

        public DateTime StartedTime { get; set; }

        public ModuleRuntmieState State { get; set; }

        public string LastLogMessage { get; set; }

        public static ModuleRuntimeDetails CreateModuleExecStateDetails(ITask task)
        {         
            if (task != null)
            {
                switch (task.State.State)
                {
                    case TaskState.Wait: return new ModuleRuntimeDetails() { Message = "Module was added to execution queue. Pleas wait...", StartedTime = task.State.StartedTime, State = ModuleRuntmieState.Wait , LastLogMessage = task.State.Message};
                    case TaskState.Running: return new ModuleRuntimeDetails() { Message = "Module is running. Pleas wait...", StartedTime = task.State.StartedTime, State = ModuleRuntmieState.Running , LastLogMessage = task.State.Message };
                    case TaskState.Completed: return new ModuleRuntimeDetails() { Message = "Module was completed successfully", StartedTime = task.State.StartedTime, State = ModuleRuntmieState.Completed, LastLogMessage = task.State.Message };
                    case TaskState.CompletedWithError: return new ModuleRuntimeDetails() { Message = "Module was completed with error", StartedTime = task.State.StartedTime, State = ModuleRuntmieState.Completed, LastLogMessage = task.State.Message };
                };
            }

            return new ModuleRuntimeDetails() { State = ModuleRuntmieState.Absent};
        }

    }

}
