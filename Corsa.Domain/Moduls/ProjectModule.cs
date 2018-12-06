using Corsa.Domain.Models.Projects;
using Corsa.Domain.Processing.Moduls;
using Corsa.Domain.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corsa.Domain.Moduls
{
    public abstract class ProjectModule<TConfig, TData> : Module, IRuntimeModule<TData>, IProjectModule<TConfig, TData>, ISchedulable , ILinkable where TConfig : ModuleСonfig where TData : ModuleData
    {   
        public TConfig Configuration { get; set; }

        public List<ModuleTaskResult<TData>> Results { get; set; }

        public int Id { get; set; }

        public bool Drop()
        {
            return Context.Repository.DropModule(Id);
        }

        public override string Name
        {
            get { return Configuration?.ProjectModule?.Name; }
        }
        public bool DropResult(int resultId)
        {
            return Context.Repository.DropModuleResult(resultId);
        }

        public ICollection<TaskSchedule> GetSchedules()
        {
            return Context.Repository.GetSchedules(Id).ToList();
        }

        public bool Load()
        {
            return LoadConfig() && LoadResults();
        }

        public ModuleTaskResult<TData> GetResult(int resultId)
        {
            return Results.SingleOrDefault(result => result.Id == resultId);
        }

        public abstract bool CreateAndSave(TConfig config);

        public abstract TData Run();

        public abstract bool LoadConfig();

        public abstract bool LoadResults();

        public abstract bool SaveConfig(TConfig config);

        public abstract bool SaveResult(ModuleTaskResult<TData> result);

        public  ICollection<LinkedModule> GetLinkedModules()
        {
            return Context.Repository.GetLinkedModules(Id);
        }

        public static List<T> MergeCollection<T>(List<T> newCollection, List<T> oldCollection, Func<T, T, bool> condition, Action<T> init)
        {
            List<T> result = new List<T>();

            foreach (var newItem in newCollection)
            {
                var oldItem = oldCollection.SingleOrDefault(item => condition(newItem, item));
                if (oldItem != null)
                {
                    result.Add(oldItem);
                }
                else
                {
                    init(newItem);
                    result.Add(newItem);
                }
            }
            return result;
        }
    }
}
