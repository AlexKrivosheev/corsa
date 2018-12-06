using Corsa.Domain.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using Corsa.Domain.Processing.Actions;

namespace Corsa.Infrastructure.Tracking
{
    public class ActionTrackerProvider : ITrackerProvider
    {        
        ISourceRepository Repository { get; set; }

        public ActionTrackerProvider(ISourceRepository repository, string user)
        {            
            this.User = user;
            this.Repository = repository;
        }

        public string User{get;set;}
        
        public ITracker CreateTreacker()
        {            
            return new UserActionDbTracker(User, Repository);
        }
    }
}
