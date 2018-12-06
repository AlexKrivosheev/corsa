using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Corsa.Domain.Moduls.RutimeModuls.Antigate
{
    [DataContract]
    [KnownType(typeof(ImageToTextTask))]
    public abstract class AntigateTaskConfig
    {
        [DataMember(Name = "type")]
        public abstract string Type { get; set; }

        public abstract Type ResultType { get;}
    }
}
