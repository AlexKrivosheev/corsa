using Corsa.Domain.Models.Actions;
using Corsa.Domain.Moduls;
using System;
using System.Collections.Generic;

namespace Corsa.Domain.Processing.Actions
{
    public class TrackerEventArgs : EventArgs
    {
        public TrackerEventArgs(ITracker tracker)
        {
            this.Tracker = tracker;
        }

        public ITracker Tracker{get;set;}
    }

    public interface ITracker : IDisposable
    {
        event EventHandler<TrackerEventArgs> Ended;

        event EventHandler<TrackerEventArgs> Began;

        UserActionDetails ActionDetails { get; set; }

        void Begin(UserActionDetails parent, UserAction action);

        void End();
    }
}
