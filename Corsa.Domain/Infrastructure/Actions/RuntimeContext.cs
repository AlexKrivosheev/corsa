using Corsa.Domain.Logging;
using Corsa.Domain.Models.Actions;
using Corsa.Domain.Models.Requests;
using Corsa.Domain.Processing.Actions;
using Corsa.Infrastructure.Tracking;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Corsa.Domain.Processing
{
    public class RuntimeContext : IDisposable
    {
        public ISourceRepository Repository { get; set; }

        public IServiceProvider Provider { get; set; }

        public IStringLocalizer Localizer { get; set; }

        private Stack<ITracker> _trackerStack = new Stack<ITracker>();

        public string User { get; set; }

        public ITrackerProvider TrackerProvider { get; set; }

        public ILogger Logger { get; set; }

        public RuntimeContext(IServiceProvider provider, ISourceRepository repository, ILogger logger, IStringLocalizer localizer, string user)
        {
            this.Localizer = localizer;
            this.Logger = logger;
            this.User = user;
            this.Provider = provider;
            this.Repository = repository;
            this.TrackerProvider = new ActionTrackerProvider(repository, User);
        }

        public void LogInform(string message, Exception exception = null)
        {
            int actionId = 0;

            if (Tracker != null)
            {
                actionId = Tracker.ActionDetails.ActionId;
            }

            Logger.Inform(actionId, message, this, exception);
        }

        public void LogWarning(string message, Exception exception = null)
        {
            int actionId = 0;

            if (Tracker != null)
            {
                actionId = Tracker.ActionDetails.ActionId;
            }

            Logger.Warning(actionId, message, this, exception);
        }

        public void LogError(string message, Exception exception = null)
        {
            int actionId = 0;

            if (Tracker != null)
            {
                actionId = Tracker.ActionDetails.ActionId;
            }

            Logger.Error(actionId, message, this, exception);
        }

        public ITracker BeginTrack(UserAction action)
        {
            var tracker = TrackerProvider.CreateTreacker();
            tracker.Began += Tracker_Began;
            tracker.Ended += Tracker_Ended;
            tracker.Begin(Tracker?.ActionDetails,action);
            return tracker;
        }

        private void Tracker_Ended(object sender, TrackerEventArgs e)
        {
            if (e.Tracker == Tracker)
            {
                _trackerStack.Pop();
            }
        }

        private void Tracker_Began(object sender, TrackerEventArgs e)
        {
            _trackerStack.Push(e.Tracker);
        }

        public ITracker Tracker
        {
            get
            {
                ITracker tracker;
                _trackerStack.TryPeek(out tracker);
                return tracker;
            }
        }

        public void EndTrack()
        {
            if (Tracker != null)
            {
                Tracker.End();
            }
        }

        public void Dispose()
        {
            this.Localizer = null;
            this.Logger = null;
            this.TrackerProvider = null;
            this.Provider = null;
        }
    }
}
