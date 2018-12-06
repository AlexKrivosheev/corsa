using Corsa.Domain.Moduls;
using Corsa.Domain.Processing.Moduls;
using System.Collections.Generic;

namespace Corsa.Models.Moduls
{
    public class ModuleViewModel<TConfig,TData> where TConfig : ModuleСonfig
    {
        public int Id { get; set; }

        public ModuleViewModel()
        {

        }

        public ModuleViewModel(TConfig config):this(int.MinValue,config)
        {
            
        }

        public ModuleViewModel(int id, TConfig config):this(id, config, null)
        {
            
        }

        public ModuleViewModel(int id, TConfig config, List<ModuleTaskResult<TData>> results)
        {
            this.Id = id;
            this.Config = config;
            this.Results = results;
        }

        public TConfig Config { get; }

        public List<ModuleTaskResult<TData>> Results { get; }
    }
}
