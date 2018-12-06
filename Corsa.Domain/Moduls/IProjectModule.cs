using Corsa.Domain.Models.Requests;
using Corsa.Domain.Processing.Moduls;

namespace Corsa.Domain.Moduls
{
    public interface IProjectModule: IModule
    {
        bool Load();

        bool Drop();
    }

    public interface IConfigModule<TConfig> : IProjectModule
    {
        bool CreateAndSave(TConfig config);
 
        bool SaveConfig(TConfig config);

        bool LoadConfig();
    }

    public interface IDataModule<TData> : IProjectModule
    {
        bool SaveResult(ModuleTaskResult<TData> result);

        bool LoadResults();

        ModuleTaskResult<TData> GetResult(int resultId);

        bool DropResult(int resultId);        
    }

    public interface IProjectModule<TConfig, TData> : IConfigModule<TConfig>, IDataModule<TData>
    {        
     
    }
}
