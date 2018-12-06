using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Processing.Actions
{
    public interface ITrackerProvider
    {       
        ITracker CreateTreacker();        
    }
}
