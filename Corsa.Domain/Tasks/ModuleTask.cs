using Corsa.Domain.Exceptions;
using Corsa.Domain.Models.Actions;
using Corsa.Domain.Moduls;
using Corsa.Domain.Processing;
using Corsa.Domain.Tasks.Modules;
using System;

namespace Corsa.Domain.Tasks
{
    public class RuntimeTaskEventArgs : EventArgs
    {
        public int Task { get; set; }
    }

    public abstract class ModuleTask : ITask
    {
        protected ModuleTask(RuntimeContext context, int id)
        {
            Context = context;
            Id = id;
        }

        public TaskStateDetails State { get; set; } = new TaskStateDetails();

        public event EventHandler<RuntimeTaskEventArgs> Started;

        public event EventHandler<RuntimeTaskEventArgs> Completed;

        public abstract bool Start();

        protected virtual void OnStarted()
        {
            State.State = TaskState.Running;
            State.StartedTime = DateTime.Now;

            if (Started != null)
            {
                Started(this, new RuntimeTaskEventArgs());
            }
        }

        protected virtual void OnCompleted(ActionExecutionResult result, string message)
        {
            if (result == ActionExecutionResult.Error)
            {
                State.State = TaskState.CompletedWithError;
                State.Message = message;
            }

            if (State.State == TaskState.Running)
            {
                State.State = TaskState.Completed;
            }

            State.FinishTime = DateTime.Now;

            if (Completed != null)
            {
                Completed(this, new RuntimeTaskEventArgs());
            }            
        }

        public bool Load()
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public RuntimeContext Context { get; set; }

        public abstract string Name { get; }

        public abstract int Code { get; }

        public int Id { get; }
    }

    public abstract class ModuleTask<TData> : ModuleTask
    {
        protected ModuleTask(RuntimeContext context, int id): base(context,id)
        {
        
        }

        protected ModuleTask(int id) : base(null,id)
        {            
            
        }

        public abstract TData RunModule();

        public virtual ModuleTaskResult<TData> Run()
        {
            var result = new ModuleTaskResult<TData>();

            using (this.Context.BeginTrack(new UserAction() { Id = Code }))
            {
                try
                {
                    this.Context.LogInform(this.Context.Localizer[$"Modul {Name} was run"]);
                    result.Data = RunModule();
                    this.Context.LogInform(this.Context.Localizer[$"Modul {Name} execution was completed"]);
                    return result;
                }
                catch (UserException exc)
                {                    
                    this.Context.Tracker.ActionDetails.Message = exc.Message;
                    this.Context.LogError(exc.Message);
                    result.Error = exc;
                }
                catch (AggregateException exc)
                {                    
                    this.Context.Tracker.ActionDetails.Message = this.Context.Localizer["Unexptected error. See logs"];
                    result.Error = exc;

                    foreach (var innerExc in exc.InnerExceptions)
                    {
                        this.Context.LogError(innerExc.Message);
                        this.Context.LogError(innerExc.InnerException?.Message);
                    }
                }
                catch (Exception exc)
                {                    
                    this.Context.Tracker.ActionDetails.Message = this.Context.Localizer["Unexptected error. See logs"];
                    this.Context.LogError(exc.Message);
                    this.Context.LogError(exc.InnerException?.Message);
                    result.Error = exc;
                }
                finally
                {
                    result.Details = this.Context.Tracker.ActionDetails;
                    if (result.Error != null)
                    {
                        this.Context.Tracker.ActionDetails.Result = ActionExecutionResult.Error;
                    }
                }
            }

            return result;
        }
    }

    public static class RuntimeTask
    {
        public const string RuntimeException = "An error occurred in runtime module \"{0}\". See log for getting more details.";
        public static ModuleTaskResult<TData> Run<TConfig, TData>(RuntimeContext context, IRuntimeModule<TConfig, TData> runtimeModule, TConfig config)
        {
            runtimeModule.Context = context;
            var task = new RuntimeModuleTask<TConfig, TData>(context, runtimeModule, config);
            return task.Run();
        }
    }
}