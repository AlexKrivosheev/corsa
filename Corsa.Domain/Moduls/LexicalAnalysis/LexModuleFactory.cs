
using Corsa.Domain.Moduls;

namespace Corsa.Domain.Processing.Moduls
{
    public class LexModuleFactory : IProjectModuleFactory
    {
        public IProjectModule CreateModule()
        {
            return new LexModule();
        }
        
        public IProjectModule OpenModule(int id)
        {
            return new LexModule(id);
        }
    }
}
