using Corsa.Domain.Logging;
using Microsoft.EntityFrameworkCore;

namespace Corsa.Infrastructure.Logging
{
    public class AppLogDbContext : DbContext
    {
        public AppLogDbContext(DbContextOptions<AppLogDbContext> options) : base(options)
        {

            
        }
        
        public DbSet<Log> Logs { get; set; }

    }
}
