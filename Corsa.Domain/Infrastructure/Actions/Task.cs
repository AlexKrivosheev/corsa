using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Processing.Actions
{
    public class Task : IDisposable
    {
        private RuntimeContext _context;
        private Action _action;

        public Task(RuntimeContext context, Action action)
        {
            _context = context;
            _action = action;
        }

        public void Run()
        {
            _action.Invoke();
        }

        public void Dispose()
        {
            
        }
    }
}
