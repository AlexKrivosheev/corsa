using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corsa.Domain.Logging
{
    public interface ILogReprository
    {
        bool AddLog(Log log);

        bool Clear(string owner);

        IQueryable<Log> GetLogs(string owner);

        IQueryable<Log> Logs { get; }
    }
}
