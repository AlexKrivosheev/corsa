using Corsa.Domain.Models.Actions;
using Corsa.Domain.Models.Requests;
using Corsa.Domain.Processing.Actions;
using System;

namespace Corsa.Infrastructure.Tracking
{
    public class UserActionDbTracker : ITracker
    {        
        public ISourceRepository Repository { get; set; }
        public UserActionDetails ActionDetails { get; set; } = new UserActionDetails();
        
        public string User { get; set; }
        
        public UserActionDbTracker(string user, ISourceRepository repository)
        {
            User = user;
            this.Repository = repository;
        }

        public event EventHandler<TrackerEventArgs> Ended;
        public event EventHandler<TrackerEventArgs> Began;

        public void Dispose()
        {
            End();
        }
  
        public void End()
        {
            this.ActionDetails.FinishedTime = DateTime.Now;

            Repository.UpdateActionStatistics(ActionDetails);

            if (Began != null)
            {
                Ended(this, new TrackerEventArgs(this));
            }
        }

        public void Begin(UserActionDetails parent, UserAction action)
        {
            this.ActionDetails.Parent = parent;
            this.ActionDetails.UserId = User;
            this.ActionDetails.ActionId = action.Id;
            this.ActionDetails.CreatedTime = DateTime.Now;
            this.ActionDetails.FinishedTime = new DateTime(2000, 1, 1);
            
            Repository.AddActionStatistics(ActionDetails);

            if (Began != null)
            {
                Began(this, new TrackerEventArgs(this));
            }
        }
    }
}
