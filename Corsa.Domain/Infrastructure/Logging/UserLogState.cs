using Corsa.Domain.Models.Actions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corsa.Domain.Processing
{
    public class UserLogState
    {
        public RuntimeContext Context { get; set; }

        public string Message { get; set; }

        public int? ActionStatisticId
        {
            get
            {
                return Context.Tracker?.ActionDetails.Id;
            }
        }



    }
}
