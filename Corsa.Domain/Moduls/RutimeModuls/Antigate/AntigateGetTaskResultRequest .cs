using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Corsa.Domain.Moduls.RutimeModuls.Antigate
{
    [DataContract]
    public class AntigateGetTaskResultRequest
    {
        public AntigateGetTaskResultRequest(string clientKey, int taskId)
        {
            ClientKey = clientKey;
            TaskId = taskId;
        }

        [DataMember(Name = "clientKey")]
        public string ClientKey { get; set; }

        [DataMember(Name = "taskId")]
        public int TaskId { get; set; }
    }
}
