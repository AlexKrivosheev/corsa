using Corsa.Domain.Models.Projects;
using Corsa.Domain.Moduls;

namespace Corsa.Domain.Processing.Moduls
{
    public interface IProjectModuleFactory
    {
        IProjectModule CreateModule();

        IProjectModule OpenModule( int id);
    }
}
