using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Tasks
{
    public enum TaskState
    {
        Wait,
        Running,
        Completed,
        CompletedWithError
    }
}
