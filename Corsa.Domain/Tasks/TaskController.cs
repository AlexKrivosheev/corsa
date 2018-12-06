using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Corsa.Domain.Tasks
{

    public sealed class ThreadCounter
    {
        private struct SafeCounter : IDisposable
        {
            private readonly ThreadCounter _counter;

            public SafeCounter(ThreadCounter counter)
            {
                _counter = counter;
                _counter.Value++;
            }

            public void Dispose()
            {
                _counter.Value--;
            }
        }

        private int _value;
        
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }
        
        public IDisposable Use()
        {
            return new SafeCounter(this);
        }
    }

    public abstract class TimedHostedService : IHostedService
    {
        public IServiceProvider Provider { get; set; }

        public Timer Timer { get; set; }

        public int Period { get; set; }

        public ThreadCounter _threadCounter = new ThreadCounter();

        public TimedHostedService(IServiceProvider provider, int period)
        {
            this.Provider = provider;
            this.Period = period;            
        }

        public virtual void Start()
        {
            this.Timer = new Timer(HandleCallback, null, 0, Period);
        }

        public virtual void Stop()
        {
            this.Timer.Dispose();
        }

        public void HandleCallback(object state)
        {
            if (_threadCounter.Value > 0)
            {
                return;
            }

            try
            {
                using (_threadCounter.Use())
                {
                    Run();
                }                
            }
            catch (Exception exc)
            {

            }

        }

        public abstract void Run();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Stop();
            return Task.CompletedTask;
        }
    }
}
