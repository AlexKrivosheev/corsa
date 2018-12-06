using Corsa.Domain.Logging;
using Corsa.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Corsa.Models
{
    public class AppLogRepository : ILogReprository
    {
        private AppLogDbContext _context;
        private object _lock = new object();

        public AppLogRepository(DbContextOptions<AppLogDbContext> options)
        {
            _context = new AppLogDbContext(options);
        }

        public IQueryable<Log> Logs => throw new NotImplementedException();

        public bool AddLog(Log log)
        {
            lock (_lock)
            {
                _context.Logs.Add(log);
                _context.SaveChanges();
                return true;
            }
        }

        public bool Clear(string owner)
        {
            lock (_lock)
            {
                _context.RemoveRange(GetLogs(owner));
                _context.SaveChanges();
            }
            return true;
        }

        public IQueryable<Log> GetLogs(string owner)
        {
            return _context.Logs.Where(log => string.Equals(log.UserId, owner));
        }
    }
}
